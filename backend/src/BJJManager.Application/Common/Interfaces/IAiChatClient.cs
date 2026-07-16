using BJJManager.Application.Chat;

namespace BJJManager.Application.Common.Interfaces;

public interface IAiChatClient
{
    Task<string> GetReplyAsync(IReadOnlyList<ChatMessageDto> messages, CancellationToken cancellationToken);
}
