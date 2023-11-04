using Newtonsoft.Json;

namespace AutoHelper.Application.Vehicles._DTOs;


public class RDWDetectedDefectDescription
{
    [JsonProperty("gebrek_identificatie")]
    public string Identification { get; set; }

    [JsonProperty("ingangsdatum_gebrek")]
    public string StartDateDefectRaw { get; set; }

    [JsonProperty("einddatum_gebrek")]
    public string EndDateDefectRaw { get; set; }

    [JsonProperty("gebrek_artikel_nummer")]
    public string DefectArticleNumber { get; set; }

    [JsonProperty("gebrek_omschrijving")]
    public string Description { get; set; }

    [JsonProperty("ingangsdatum_gebrek_dt")]
    public DateTime StartDateDefect { get; set; }

    [JsonProperty("einddatum_gebrek_dt")]
    public DateTime EndDateDefect { get; set; }
}