using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire.Dashboard;
using Microsoft.Extensions.Hosting;

namespace AutoHelper.Hangfire;

public class HangfireDashboardAuthFilter : IDashboardAuthorizationFilter
{
    private readonly IHostEnvironment _env;
    private const string RequiredUsername = "User1";
    private const string RequiredPassword = "Autohelper123!";

    public HangfireDashboardAuthFilter(IHostEnvironment env)
    {
        _env = env;
    }

    public bool Authorize(DashboardContext context)
    {
        // Skip authorization if environment is Development
        if (_env.IsDevelopment())
        {
            return true;
        }

        var httpContext = context.GetHttpContext();
        string authHeader = httpContext.Request.Headers["Authorization"];

        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Basic "))
        {
            return false;
        }

        string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
        var encoding = Encoding.GetEncoding("iso-8859-1");
        string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

        int separatorIndex = usernamePassword.IndexOf(':');

        var username = usernamePassword.Substring(0, separatorIndex);
        var password = usernamePassword.Substring(separatorIndex + 1);

        return username == RequiredUsername && password == RequiredPassword;
    }
}
