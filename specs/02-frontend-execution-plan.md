# Frontend Execution Plan — BJJManager Angular App

Implements the frontend slice of `00-master-spec.md`. References the API contract in that
document's §4 — do not redefine routes/DTOs here.

## 1. Project layout (Angular 18, standalone components, monorepo path `/frontend`)

```
/frontend
  /bjj-manager-app
    src/app/
      core/
        auth/            # auth.service.ts, jwt.interceptor.ts, auth.guard.ts (functional), token.storage.ts
        models/          # TS interfaces mirroring backend DTOs (User, TrainingSession, Technique, ...)
      features/
        auth/            # login.component.ts, register.component.ts
        dashboard/        # dashboard.component.ts, dashboard.service.ts
        trainings/         # training-list, training-form (shared for create/edit), training.service.ts
        techniques/        # technique-list, technique-form (dynamic steps), technique.service.ts
      shared/
        components/       # navbar (welcome message), confirm-dialog, loading-spinner
      app.routes.ts
      app.config.ts
    src/environments/
      environment.ts        # apiBaseUrl: http://localhost:5xxx/api
      environment.development.ts
```

## 2. Pages (per master spec §"UI"/ProjectIdea)

Login, Register, Dashboard, Training Sessions (list w/ filters), Create Training, Edit Training,
Techniques (list), Create Technique, Edit Technique — one route each, guarded except Login/Register.

## 3. Build order

1. **Scaffold**: `ng new bjj-manager-app --standalone --routing --style=scss`, add
   `provideHttpClient(withInterceptors([jwtInterceptor]))` in `app.config.ts`.
2. **Core/auth**:
   - `AuthService`: `register()`, `login()` (stores JWT in memory + `localStorage`, decodes
     `exp`/`name` claim for the welcome banner), `logout()`, `isAuthenticated` (signal).
   - `jwtInterceptor` (functional `HttpInterceptorFn`): attaches `Authorization: Bearer <token>`;
     on 401 response, clears token and redirects to `/login`.
   - `authGuard` (functional `CanActivateFn`): redirects to `/login` if not authenticated.
3. **Login/Register components**: Reactive Forms; Register validates password ≥6 chars
   client-side (mirrors backend rule, backend remains source of truth); on success, Login
   navigates to `/dashboard`.
4. **Shared navbar**: shows "Welcome, {name}!" once authenticated, logout button, nav links.
5. **Dashboard**: calls `GET /api/dashboard`, renders welcome message + totals as simple stat
   tiles.
6. **Trainings**:
   - `TrainingService`: thin wrapper over `HttpClient` for the six training endpoints in master
     spec §4.2.
   - List component: table/cards, filter controls (exact date picker, month, year — client sends
     only the filter(s) the user actually set), edit/delete actions per row.
   - Form component (shared create/edit): date, academy name, notes (optional), self-evaluation
     (1–5, rendered as labeled radio/select using the meaning table from the master spec, not bare
     numbers), plus a simple repeatable "media" sub-form (fileName, fileUrl, mediaType) — add/remove
     rows client-side, submitted as part of create, or via the dedicated media endpoints on edit.
7. **Techniques**:
   - `TechniqueService`: wrapper over the technique endpoints.
   - List component: shows name + position, link to edit.
   - Form component: name, position (dropdown of the example positions + "Custom…" free-text
     fallback, per master spec §3.4), description, and a **dynamic ordered step list** — add step,
     remove step (disabled/blocked when only one step remains, matching "first step mandatory, at
     least one must always exist"), drag-or-button-based reordering that recomputes `Order` before
     submit. Media sub-form same shape as Trainings.
8. **Error handling**: a small `ErrorHandlingService` or interceptor maps backend `ProblemDetails`
   (400 field errors, 401, 404) to form field errors / toast notifications, consistent across all
   feature forms.
9. **Testing**: Jasmine/Karma (Angular CLI default) — unit tests for `AuthService`,
   `jwtInterceptor`, `authGuard`, and each feature service (HTTP mocked via
   `HttpTestingController`); component tests for the two form components covering the validation
   rules above (password length, self-evaluation range, ≥1 step retained).

## 4. State management

Deliberately no NgRx — scope doesn't warrant it. Per-feature services hold state as Angular
signals (`signal()`/`computed()`), matching Angular 18 idioms and the "simple, not complex"
directive. Components read/write via the service, not directly against `HttpClient`.

## 5. Styling

Angular Material: fastest path to "responsive, clean, simple" (master spec's UI requirement)
without hand-rolling a design system, and it composes fine with Reactive Forms and signals. Plain
SCSS on top for layout only (flex/grid page shells).

## 6. Cross-references

- API base URL / CORS origin must match what `01-backend-execution-plan.md` configures.
- Firebase web config in `Chaves e StringConnections.txt` powers the media upload flow: the
  Firebase SDK uploads the file client-side to Storage and the app persists the returned download
  URL through the training/technique media endpoints.

## 7. Definition of done for this slice

- All 9 pages implemented and reachable via routing, guards enforced.
- `ng test` green.
- No console warnings/errors in the browser during the golden-path flow (register → login →
  create training → create technique with steps → dashboard totals update).
- No code comments anywhere in `/frontend`.
