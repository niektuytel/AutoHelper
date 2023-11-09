using Newtonsoft.Json;

namespace AutoHelper.Application.Vehicles._DTOs;

public class RDWVehicleBasics
{
    [JsonProperty("kenteken")]
    public string LicensePlate { get; set; }

    [JsonProperty("europese_voertuigcategorie")]
    public string EuropeanVehicleCategory { get; set; }

    [JsonProperty("vervaldatum_apk_dt")]
    public DateTime MOTExpiryDateDt { get; set; }

    [JsonProperty("datum_tenaamstelling_dt")]
    public DateTime RegistrationDateDt { get; set; }
}