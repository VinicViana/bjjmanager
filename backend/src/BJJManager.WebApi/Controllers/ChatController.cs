using BJJManager.Application.Chat;
using BJJManager.WebApi.Contracts.Chat;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BJJManager.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly ISender _sender;

    public ChatController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> Send(SendChatRequest request, CancellationToken cancellationToken)
    {
        var messages = request.Messages.Select(m => new ChatMessageDto(m.Role, m.Content)).ToList();
        var result = await _sender.Send(new SendChatMessageCommand(messages), cancellationToken);
        return Ok(result);
    }
}
