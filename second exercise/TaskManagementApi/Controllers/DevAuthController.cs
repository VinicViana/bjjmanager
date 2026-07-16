using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.Auth;

namespace TaskManagementApi.Controllers;

[ApiController]
[Route("api/dev-auth")]
public class DevAuthController : ControllerBase
{
    private readonly DevTokenGenerator _tokenGenerator;
    private readonly IWebHostEnvironment _environment;

    public DevAuthController(DevTokenGenerator tokenGenerator, IWebHostEnvironment environment)
    {
        _tokenGenerator = tokenGenerator;
        _environment = environment;
    }

    [HttpPost("token")]
    public ActionResult<object> IssueToken([FromQuery] Guid? userId)
    {
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        var effectiveUserId = userId ?? Guid.Parse("11111111-1111-1111-1111-111111111111");
        var token = _tokenGenerator.GenerateToken(effectiveUserId);

        return Ok(new { userId = effectiveUserId, token });
    }
}
