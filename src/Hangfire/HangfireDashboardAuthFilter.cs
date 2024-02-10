using Hangfire.Dashboard;

namespace AutoHelper.Hangfire;

public class HangfireDashboardAuthFilter : IDashboardAuthorizationFilter
{
    private const string RequiredUsername = "Admin";
    private const string RequiredPassword = "Auto1337!";
    private readonly bool _inDevelopment;

    public HangfireDashboardAuthFilter(bool inDevelopment)
    {
        _inDevelopment = inDevelopment;
    }

    public bool Authorize(DashboardContext context)
    {
        // Skip authorization if environment is Development
        if (_inDevelopment)
        {
            return true;
        }

        var httpContext = context.GetHttpContext();

        // Try to get the username and password from the request
        string authHeader = httpContext.Request.Headers["Authorization"];
        if (string.IsNullOrWhiteSpace(authHeader))
        {
            return false;
        }

        var credentials = GetCredentials(authHeader);
        if (credentials == null)
        {
            return false;
        }

        // Check if the username and password match
        return credentials.Item1 == RequiredUsername && credentials.Item2 == RequiredPassword;
    }

    private Tuple<string, string> GetCredentials(string authHeader)
    {
        if (!authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        string encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
        string decodedCredentials;

        try
        {
            decodedCredentials = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
        }
        catch
        {
            return null;
        }

        var parts = decodedCredentials.Split(':');
        if (parts.Length != 2)
        {
            return null;
        }

        return new Tuple<string, string>(parts[0], parts[1]);
    }
}
