using Newtonsoft.Json;

namespace AutoHelper.Infrastructure.Common.Models;

public class RDWKnownService
{
    [JsonProperty("volgnummer")]
    public int Volgnummer { get; set; }

    [JsonProperty("erkenning")]
    public string Erkenning { get; set; }
}