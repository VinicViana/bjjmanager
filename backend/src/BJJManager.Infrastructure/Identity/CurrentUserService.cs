using System.Security.Claims;
using BJJManager.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BJJManager.Infrastructure.Identity;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            return value is not null && Guid.TryParse(value, out var userId) ? userId : Guid.Empty;
        }
    }
}
