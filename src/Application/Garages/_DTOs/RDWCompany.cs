using System.Globalization;
using Newtonsoft.Json;

namespace AutoHelper.Application.Garages._DTOs;

public class RDWCompany
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

    public string GetFormattedAddress()
    {
        var street = Straat;
        var houseNumber = Huisnummer.ToString();
        var houseNumberAddition = Huisnummertoevoeging;

        if (string.IsNullOrWhiteSpace(street))
        {
            throw new Exception("Street is empty");
        }

        if (string.IsNullOrWhiteSpace(houseNumber))
        {
            throw new Exception("House number is empty");
        }

        // Capitalize the first letter of the street
        street = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(street.ToLower());

        // Remove unexpected commas from the inputs
        street = street.Replace(",", "").Trim();
        houseNumber = houseNumber.Replace(",", "").Trim();
        houseNumberAddition = string.IsNullOrWhiteSpace(houseNumberAddition) ? "" : houseNumberAddition.Replace(",", "").Trim();

        // Conditionally add comma based on the presence of houseNumber
        if (!string.IsNullOrEmpty(houseNumber))
        {
            return $"{street} {houseNumber}{houseNumberAddition}";
        }
        else
        {
            return $"{street} {houseNumberAddition}".Trim();
        }
    }
}