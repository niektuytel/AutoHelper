using Newtonsoft.Json;

namespace AutoHelper.Application.Vehicles._DTOs;


public class VehicleDetectedDefectDtoItem
{
    public VehicleDetectedDefectDtoItem()
    {

    }

    [JsonProperty("kenteken")]
    public string LicensePlate { get; set; }

    [JsonProperty("soort_erkenning_keuringsinstantie")]
    public string TypeOfRecognitionInspection { get; set; }

    [JsonProperty("meld_datum_door_keuringsinstantie_dt")]
    public DateTime DetectionDate { get; set; }

    [JsonProperty("gebrek_identificatie")]
    public string Identifier { get; set; }

    [JsonProperty("soort_erkenning_omschrijving")]
    public string Description { get; set; }

    [JsonProperty("aantal_gebreken_geconstateerd")]
    public int DetectedAmount { get; set; }
}
