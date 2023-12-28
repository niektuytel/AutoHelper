using System.Security.Claims;

using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Infrastructure.Common.Interfaces;
using AutoHelper.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace AutoHelper.Hangfire.Dashboard;

public class CurrentUserService : ICurrentUserService
{
    public CurrentUserService()
    {
    }

    public string? UserId => "AutoHelper.Hangfire.WebUI.UserId";

    public string? UserName => "AutoHelper.Hangfire.WebUI.UserName";

    public string? UserEmail => "AutoHelper.Hangfire.WebUI.UserEmail";

}
