using AutoHelper.Domain.Entities.Garages;
using Newtonsoft.Json;

namespace AutoHelper.Infrastructure.Common.Models;

public class RDWKnownService
{
    [JsonProperty("volgnummer")]
    public int Volgnummer { get; set; }

    [JsonProperty("erkenning")]
    public string Erkenning { get; set; }

    [JsonIgnore]
    public GarageServiceType ServiceType { get; set; }
}