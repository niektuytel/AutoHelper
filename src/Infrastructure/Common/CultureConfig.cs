using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
