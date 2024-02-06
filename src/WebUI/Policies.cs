namespace AutoHelper.WebUI;

internal static class Policies
{
    internal const string AdminDefaultPolicy = "AdminDefaultPolicy";
    internal const string GarageDefaultPolicy = "GarageDefaultPolicy";
    internal const string UserDefaultPolicy = "UserDefaultPolicy";

    private const string AdminReadWrite = "Admin.ReadWrite"; // Has to be a Role not a scope
    private const string GarageReadWrite = "Garage.ReadWrite";
    private const string UserReadWrite = "User.ReadWrite";

    internal static readonly Dictionary<string, string> AllPolicies = new()
    {
        { GarageDefaultPolicy, GarageReadWrite },
        { UserDefaultPolicy, UserReadWrite }
    };

    internal static readonly Dictionary<string, string> AllScopes = new()
    {
        { $"https://autohelperb2c.onmicrosoft.com/api/{GarageReadWrite}", "Garage Control" },
        { $"https://autohelperb2c.onmicrosoft.com/api/{UserReadWrite}", "User Control" }
    };
}

