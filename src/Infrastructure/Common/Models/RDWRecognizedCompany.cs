using Newtonsoft.Json;

namespace AutoHelper.Infrastructure.Common.Models;

public class RDWService
{
    public class RDWRecognizedCompany
    {
        [JsonProperty("volgnummer")]
        public int Volgnummer { get; set; }

        [JsonProperty("naam_bedrijf")]
        public string Naambedrijf { get; set; }

        [JsonProperty("gevelnaam")]
        public string Gevelnaam { get; set; }

        [JsonProperty("straat")]
        public string Straat { get; set; }

        [JsonProperty("huisnummer")]
        public int Huisnummer { get; set; }

        [JsonProperty("huisnummer_toevoeging")]
        public string Huisnummertoevoeging { get; set; }

        [JsonProperty("postcode_numeriek")]
        public int Postcodenumeriek { get; set; }

        [JsonProperty("postcode_alfanumeriek")]
        public string Postcodealfanumeriek { get; set; }

        [JsonProperty("plaats")]
        public string Plaats { get; set; }

        [JsonProperty("api_bedrijf_erkenningen")]
        public string ApiBedrijfErkenningen { get; set; }
    }

}