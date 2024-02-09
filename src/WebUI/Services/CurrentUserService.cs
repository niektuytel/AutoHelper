using System.Security.Claims;

using AutoHelper.Application.Common.Interfaces;

namespace AutoHelper.WebUI.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub");

    public string? UserName => _httpContextAccessor.HttpContext?.User?.FindFirstValue("given_name");

    public string? UserEmail => _httpContextAccessor.HttpContext?.User?.FindFirstValue("emails");

}
