using Newtonsoft.Json;

namespace WebUI.Models.Response;

public class LicencePlateBriefResponse
{
    public LicencePlateBriefResponse(string licencePlate)
    {
        LicencePlate = licencePlate;
    }

    [JsonProperty("licence_plate")]
    public string LicencePlate { get; }
}
