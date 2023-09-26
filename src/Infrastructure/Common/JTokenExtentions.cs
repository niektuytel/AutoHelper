using Newtonsoft.Json.Linq;

namespace AutoHelper.Infrastructure.Common;

public static class JTokenExtentions
{

    public static decimal GetSafeDecimalValue(this JToken data, string key)
    {
        if (data[key] != null)
        {
            if (decimal.TryParse(data[key].ToString(), out var value))
            {
                return value;
            }
        }

        return 0;
    }

    public static int GetSafeDateYearValue(this JToken data, string key)
    {
        if (data[key] != null)
        {
            if (DateTime.TryParse(data[key].ToString(), out var value))
            {
                return value.Year;
            }
        }

        return 0;
    }

    public static string GetSafeDateValue(this JToken data, string key)
    {
        if (data[key] != null)
        {
            if (DateTime.TryParse(data[key].ToString(), out DateTime dateValue))
            {
                return dateValue.ToString("dd-MM-yyyy");
            }
        }
        return "Niet geregistreerd";
    }

    public static string GetSafeValue(this JToken data, string key)
    {
        if (data[key] != null)
        {
            return data[key]!.ToString();
        }

        return "Niet geregistreerd";
    }
}
