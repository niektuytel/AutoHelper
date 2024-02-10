using System.Globalization;

namespace AutoHelper.Infrastructure.Common;
public static class CultureConfig
{
    public static void SetGlobalCultureToNL()
    {
        CultureInfo culture = CultureInfo.CreateSpecificCulture("nl-NL");

        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
    }
}
