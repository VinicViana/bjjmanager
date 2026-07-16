# AI Chat Execution Plan — BJJManager

Implements the AI Chat module of `00-master-spec.md` §2.5. A single-purpose, ephemeral Q&A
assistant for jiu-jitsu questions, backed by the OpenAI API.

## 1. Non-negotiables

- The OpenAI API key never reaches the browser. It lives in backend configuration only
  (`appsettings.Development.json` → `OpenAI:ApiKey`, sourced from `Chaves e StringConnections.txt`
  locally, same pattern as the SQL connection string and JWT key); the frontend calls
  `POST /api/chat` on our own API, never `api.openai.com` directly.
- Nothing about a conversation is persisted: no database table, no server-side session/cache, no
  frontend localStorage/sessionStorage. The transcript exists only in the floating widget's
  in-memory state — it survives in-app navigation between pages (the widget lives in the app shell,
  not on a routed page) but is gone on a full page reload or logout.
- Scope is jiu-jitsu only — enforced via the system prompt, not a keyword filter.
- Always responds in the same language the question was asked in — also enforced via the system
  prompt (modern OpenAI chat models follow this reliably; no separate language-detection step is
  needed).

## 2. System prompt (fixed, not user-editable)

```
You are a black belt Brazilian Jiu-Jitsu instructor with decades of experience training and
competing. Your purpose is to help with Brazilian Jiu-Jitsu — techniques, positions, strategy,
rules, training methodology, competition, belt progression, and related topics.

Greetings and small talk (like "hi", "hello", "how are you", "who are you") deserve a warm, brief,
natural reply that welcomes the person and invites them to ask a jiu-jitsu question — never the
refusal below for these.

Only if asked something clearly unrelated to jiu-jitsu (a different subject entirely), politely
say you can only help with jiu-jitsu topics and steer the conversation back.

Always respond in the same language the question was written in, regardless of what language this
instruction is in.

Keep answers complete but simple: cover what the person actually needs to know, in plain language,
without unnecessary jargon. If you do use a technical term, briefly explain it.
```

Sent once per request as the `system` message, ahead of the conversation history — never exposed
to or editable by the client.

## 3. Backend

### 3.1 Configuration
- `appsettings.Development.json` → new `OpenAI` section: `{ "ApiKey": "...", "Model": "gpt-4o-mini" }`.
- The key value comes from `Chaves e StringConnections.txt` — never hardcoded, never committed as a
  literal outside that gitignored file and the local `appsettings.Development.json`
  (`appsettings.json` keeps an empty placeholder, same treatment as `Jwt:Key`).
- Bound via `IOptions<OpenAiSettings>` (mirrors the existing `JwtSettings` pattern), registered in
  `BJJManager.Infrastructure/DependencyInjection.cs`.

### 3.2 Application layer
- `SendChatMessageCommand(IReadOnlyList<ChatMessageDto> Messages) : IRequest<ChatMessageDto>` —
  `Messages` is the running transcript the client already holds (each item
  `{ Role: "user"|"assistant", Content: string }`), ending with the new user message. The handler
  prepends the fixed system prompt, calls `IAiChatClient.GetReplyAsync(...)`, and returns the
  assistant's reply as `ChatMessageDto("assistant", replyText)`.
- Validator: at least one message; the last message must have `Role == "user"`; every `Content` is
  non-empty and capped at a reasonable length (e.g. 2,000 chars) — a cheap guard against runaway
  token costs, not a hard product requirement.
- `IAiChatClient` interface in `Application/Common/Interfaces`
  (`Task<string> GetReplyAsync(IReadOnlyList<ChatMessageDto> messages, CancellationToken ct)`) —
  keeps Application ignorant of OpenAI specifics, consistent with the rest of the codebase's
  dependency direction (Application depends only on abstractions).

### 3.3 Infrastructure layer
- `OpenAiChatClient : IAiChatClient` — uses a named `HttpClient` (`IHttpClientFactory`, base
  address `https://api.openai.com/v1/`, `Authorization: Bearer {ApiKey}` header) to call
  `POST /chat/completions` with the configured model, the system prompt, and the mapped message
  history. Deserializes `choices[0].message.content` as the reply.
- No retry/streaming for v1 — a single request/response call. Timeouts and 4xx/5xx from OpenAI
  bubble up as an `ExternalServiceException`, mapped by the existing exception middleware to a 502
  with a friendly message ("The AI coach is unavailable right now, try again in a moment.").

### 3.4 WebApi layer
- `ChatController` — `[Authorize]`, `POST /api/chat`, body `{ messages: [{ role, content }, ...] }`,
  returns `200 { role: "assistant", content: string }`. Authenticated like every other endpoint
  except `/api/auth/*` — no anonymous access, since each call costs real money on the OpenAI side.

## 4. Frontend

No dedicated route. The chat is a floating widget, present on every authenticated screen — not
something you navigate to.

- `ChatWidgetComponent` is mounted once, directly in `AppComponent`, as a sibling to the navbar and
  sidenav (`@if (isAuthenticated())`), not inside `router-outlet`. Because it lives at the app-shell
  level rather than on a routed page, it is never destroyed by in-app navigation — going from
  Dashboard to Trainings to Techniques does not reset it. It only disappears on logout, and its
  conversation only clears on a full page reload or the explicit "Clear chat" action — still
  nothing durable, just longer-lived in-memory state than a per-route component would have.
- Collapsed state: a circular floating action button fixed to the bottom-right corner
  (`position: fixed`, high `z-index`, clear of the sidenav/toolbar), icon `smart_toy` or `chat`,
  visible above whatever page content is underneath.
- Expanded state: clicking the button opens a compact floating panel anchored to that same corner
  (not a full-page takeover) — message list (user messages right-aligned, assistant left-aligned,
  matching the existing card/chip visual language), a text input + send button, a loading state
  (disable input, show a typing indicator) while awaiting the response, auto-scroll to the latest
  message, and a close control that collapses it back to just the button without clearing the
  conversation.
- On mobile widths the panel expands to near-full width/height instead of a fixed small box, same
  interaction otherwise.
- A signal on the component holds `ChatMessage[]` (`{ role, content }`), starting empty on first
  mount for the session — no persistence, no resuming a previous conversation after a reload.
- `ChatService.send(messages: ChatMessage[])` — thin wrapper posting to `/api/chat`, returns the
  assistant `ChatMessage` to append to the signal.
- Each send posts the *entire* transcript so far (nothing is persisted server-side, so the client
  is the only place the running context exists) — capped client-side to roughly the last 20
  messages before sending, to keep token usage/latency bounded on long sessions.
- A "Clear chat" button resets the signal to empty — the only way to "delete" history, since
  there's nothing to delete anywhere else.

## 5. Definition of done for this slice

- `POST /api/chat` requires a valid JWT; rejects an empty/malformed message list with 400.
- Asking a jiu-jitsu question in Portuguese gets a Portuguese answer; in English gets an English
  answer — without the client specifying a language anywhere.
- Asking something unrelated to jiu-jitsu gets a polite redirect, not an answer to the off-topic
  question.
- The floating button is visible on every authenticated screen (dashboard, trainings, techniques)
  and opens the same panel/conversation regardless of which page it's opened from; navigating
  between pages does not reset an in-progress conversation, but a full page reload or logout does.
- No chat content appears in the database, in browser storage, or in any log beyond standard
  request logging.
- Unit tests: `SendChatMessageCommandValidator` (empty list, last-message-must-be-user, length
  cap), `SendChatMessageCommandHandler` against a mocked `IAiChatClient`. No test hits the real
  OpenAI API — `IAiChatClient` is mocked/faked everywhere in the test suite.
- No code comments, consistent with the rest of the codebase.
