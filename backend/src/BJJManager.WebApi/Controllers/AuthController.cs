using BJJManager.Application.Auth;
using BJJManager.WebApi.Contracts.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BJJManager.WebApi.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new RegisterUserCommand(request.Name, request.Password), cancellationToken);
        return CreatedAtAction(nameof(Register), new { result.Id }, result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new LoginUserCommand(request.Name, request.Password), cancellationToken);
        return Ok(result);
    }
}
