namespace BJJManager.WebApi.Contracts.Chat;

public record SendChatRequest(IReadOnlyList<ChatMessageRequest> Messages);
