using Newtonsoft.Json.Linq;

namespace WebUI.Extensions;

public static class JTokenExtentions
{
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
