using Newtonsoft.Json;

namespace AutoHelper.Application.Vehicles._DTOs;

public class VehicleInspectionNotificationDtoItem
{
    [JsonProperty("kenteken")]
    public string LicensePlate { get; set; }

    [JsonProperty("soort_erkenning_keuringsinstantie")]
    public string AuthorityType { get; set; }

    [JsonProperty("soort_erkenning_omschrijving")]
    public string AuthorityDescription { get; set; }

    [JsonProperty("soort_melding_ki_omschrijving")]
    public string Description { get; set; }

    [JsonProperty("meld_datum_door_keuringsinstantie_dt")]
    public DateTime DateTimeByAuthority { get; set; }

    [JsonProperty("vervaldatum_keuring_dt")]
    public DateTime ExpiryDateTime { get; set; }

    [JsonProperty("api_gebrek_constateringen")]
    public string ApiDeficiencyFindings { get; set; }

    [JsonProperty("api_gebrek_beschrijving")]
    public string ApiDeficiencyDescription { get; set; }
}
