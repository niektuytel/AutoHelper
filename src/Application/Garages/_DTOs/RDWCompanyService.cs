using AutoHelper.Domain.Entities.Garages;
using Newtonsoft.Json;

namespace AutoHelper.Application.Garages._DTOs;

public class RDWCompanyService
{
    [JsonProperty("volgnummer")]
    public int Volgnummer { get; set; }

    [JsonProperty("erkenning")]
    public string Erkenning { get; set; }

    [JsonIgnore]
    public List<GarageLookupServiceItem> RelatedServiceItems { get; set; }
}