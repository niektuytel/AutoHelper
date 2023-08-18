using System.Diagnostics.Metrics;
using AutoHelper.Application.TodoLists.Commands.CreateTodoList;
using AutoHelper.Application.TodoLists.Commands.DeleteTodoList;
using AutoHelper.Application.TodoLists.Commands.UpdateTodoList;
using AutoHelper.Application.TodoLists.Queries.ExportTodos;
using AutoHelper.Application.TodoLists.Queries.GetTodos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WebUI.Models.Response;
using YamlDotNet.Core.Tokens;

namespace AutoHelper.WebUI.Controllers;

public class VehicleController : ApiControllerBase
{
    [HttpGet("search")]
    public async Task<ActionResult<LicencePlateBriefResponse>> SearchVehicle([FromQuery] string licensePlate)
    {
        licensePlate = licensePlate.Replace("-", "").ToUpper();
        var url = $"https://opendata.rdw.nl/resource/5xwu-cdq3.json?kenteken={licensePlate}";

        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("X-App-Token", "OKPXTphw9Jujrm9kFGTqrTg3x");
        request.Headers.Add("Accept", "application/json");
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var data = JArray.Parse(json);
        if (data.HasValues)
        {
            licensePlate = data.First().Value<string>("kenteken")!;
            var rsponse200 = new LicencePlateBriefResponse(licensePlate);
            return Ok(rsponse200);
        }

        var response404 = new LicencePlateBriefResponse(licensePlate);
        return NotFound(response404);
    }

    [HttpGet("information")]
    public async Task<ActionResult<VehicleInformationResponse>> GetVehicleInformation([FromQuery] string licensePlate)
    {
        licensePlate = licensePlate.Replace("-", "").ToUpper();
        var result = new VehicleInformationResponse();

        // Like from: https://ovi.rdw.nl/# on 87-GRN-6
        var data = await GetVehicle(licensePlate);
        if (data.HasValues)
        {
            // Basis
            #region General
            result.Data.Add(new VehicleInformationSection("Algemeen")
            {
                Values = new List<VehicleInformationSectionValue> {
                    new("Voertuigcategorie", value: data["europese_voertuigcategorie"].ToString()),
                    //new("Carrosserietype", value: $"Hatchback (AB)"),
                    new("Inrichting", value: data["inrichting"].ToString()),
                    new("Merk", value: data["merk"].ToString()),
                    new("Type", value: data["type"].ToString()),
                    new("Variant", value: data["variant"].ToString()),
                    new("Uitvoering", value: data["uitvoering"].ToString()),
                    new("Kleur", value: data["eerste_kleur"].ToString()),
                    new("Handelsbenaming", value: data["handelsbenaming"].ToString()),
                    new("Typegoedkeuringsnummer", value: data["typegoedkeuringsnummer"].ToString()),
                    new("Plaats chassisnummer", value: data["plaats_chassisnummer"].ToString()),
                    // The is no registered value for this owners amount: '4/0'
                    new("Aantal eigenaren privé / zakelijk", value: "Niet geregistreerd"),
                }
            });
            #endregion

            #region Expiration dates and history
            result.Data.Add(new VehicleInformationSection("Vervaldata en historie")
            {
                Values = new List<VehicleInformationSectionValue> {
                    new("Vervaldatum APK", value: DateTime.Parse(data["vervaldatum_apk_dt"].ToString()).ToString("dd-MM-yyyy")),
                    new("Datum eerste tenaamstelling in Nederland", value: DateTime.Parse(data["datum_eerste_tenaamstelling_in_nederland_dt"].ToString()).ToString("dd-MM-yyyy")),
                    new("Datum eerste toelating", value: DateTime.Parse(data["datum_eerste_toelating_dt"].ToString()).ToString("dd-MM-yyyy")),
                    new("Datum inschrijving voertuig in Nederland", value: DateTime.Parse(data["datum_eerste_toelating_in_nederland_dt"].ToString()).ToString("dd-MM-yyyy")),
                    // There is no "Registratie datum goedkeuring" in the provided JSON, so we set a default value
                    new("Registratie datum goedkeuring", value: "Niet geregistreerd"),
                    new("Datum laatste tenaamstelling", value: DateTime.Parse(data["datum_tenaamstelling_dt"].ToString()).ToString("dd-MM-yyyy")),
                    // There's no separate "Tijdstip laatste tenaamstelling" field in the JSON. Using the time part of "datum_tenaamstelling_dt"
                    new("Tijdstip laatste tenaamstelling", value: "Niet geregistreerd"),
                }
            });
            #endregion

            #region Weights
            result.Data.Add(new VehicleInformationSection("Gewichten")
            {
                Values = new List<VehicleInformationSectionValue> {
                    new("Massa rijklaar", value: $"{data["massa_rijklaar"]} kg"),
                    new("Massa ledig voertuig", value: $"{data["massa_ledig_voertuig"]} kg"),
                    new("Technische max. massa voertuig", value: $"{data["technische_max_massa_voertuig"]} kg"),
                    new("Toegestane max. massa voertuig", value: $"{data["toegestane_maximum_massa_voertuig"]} kg"),
                    new("Maximum massa samenstel", value: $"{data["maximum_massa_samenstelling"]} kg"),
                    new("Aanhangwagen geremd", value: $"{data["maximum_trekken_massa_geremd"]} kg"),
                    new("Aanhangwagen ongeremd", value: $"{data["maximum_massa_trekken_ongeremd"]} kg"),
                }
            });
            #endregion

            #region Counter readings
            result.Data.Add(new VehicleInformationSection("Tellerstanden")
            {
                Values = new List<VehicleInformationSectionValue> {
                    new("Jaar laatste registratie", value: $"{data["jaar_laatste_registratie_tellerstand"]}"),
                    new("Oordeel", value: $"{data["tellerstandoordeel"]}"),
                    new("Toelichting", value: GetCounterReadingsDescription(data["code_toelichting_tellerstandoordeel"].ToString()))
                }
            }); ;
            #endregion

            #region Status of the vehicle
            result.Data.Add(new VehicleInformationSection("Status van het voertuig")
            {
                Values = new List<VehicleInformationSectionValue> {
                    new("Gestolen", value: $"Niet geregistreerd"),  // Replace with actual data extraction logic if available
                    new("Geëxporteerd", value: $"{data["export_indicator"]}"), // Assuming "export_indicator" field holds this value
                    new("WAM verzekerd", value: $"{data["wam_verzekerd"]}"),
        
                    // Not sure which field from JSON indicates "Verbod voor rijden op de weg", for demonstration, I've left it as static "Nee"
                    new("Verbod voor rijden op de weg", value: "Niet geregistreerd"),  // Replace with actual data extraction logic if available
        
                    new("Tenaamstellen mogelijk", value: $"{data["tenaamstellen_mogelijk"]}")
                }
            });
            #endregion

            #region Recall
            string terugroepactieStatus = data["openstaande_terugroepactie_indicator"].ToString() == "Nee"
                ? "Geen terugroepactie(s) geregistreerd"
                : "Er zijn openstaande terugroepactie(s)"; // Adjust based on the possible values in the dataset

            result.Data.Add(new VehicleInformationSection("Terugroepacties")
            {
                Values = new List<VehicleInformationSectionValue> {
                    new("Status terugroepactie(s)", value: terugroepactieStatus),
                }
            });
            #endregion

            // Moter & milleu
            #region Moter
            result.Data.Add(new VehicleInformationSection("Moter")
            {
                Values = new List<VehicleInformationSectionValue> {
                    new("Cilinderinhoud", value: $"{data["cilinderinhoud"]} cm³"),
                    new("Aantal cilinders", value: data["aantal_cilinders"].ToString()),
                    new("Type gasinstallatie", value: "Niet van toepassing"),
                    new("Emissieklasse diesel", value: "Niet van toepassing"),
                }
            });
            #endregion

            //#region Environmental performance
            //result.Data.Add(new VehicleInformationSection("Milleuprestaties")
            //{
            //    Values = new List<VehicleInformationSectionValue> {
            //        new("Brandstof", value: "Benzine"),
            //        new("Brandstofverbruik NEDC", value: "7 l/100km"),
            //        new("Brandstofverbruik WLTP", value: "Niet geregistreerd"),
            //        new("Elektrisch verbruik NEDC", value: "Niet geregistreerd"),
            //        new("Elektrisch verbruik WLTP", value: "Niet geregistreerd"),
            //        new("Elektrische actieradius NEDC", value: "Niet geregistreerd"),
            //        new("Elektrische actieradius WLTP", value: "Niet geregistreerd"),
            //        new("Geluidsniveau stationair", value: "83 dB(A)"),
            //        new("Geluidsniveau toerental motor", value: "4125 min-1"),
            //        new("Geluidsniveau rijdend", value: "73 dB(A)"),
            //        new("Nettomaximumvermogen", value: "55 kW"),
            //        new("Nominaal continu maximumvermogen", value: "Niet geregistreerd"),
            //        new("Maximum vermogen 60 minuten", value: "Niet geregistreerd"),
            //        new("Netto maximumvermogen elektrisch", value: "Niet geregistreerd"),
            //    }
            //});

            //#region Emissions
            //result.Data.Add(new VehicleInformationSection("Uitstoot")
            //{
            //    Values = new List<VehicleInformationSectionValue> {
            //        new("Brandstof", value: "Benzine"),
            //        new("Uitstoot deeltjes (licht)", value: "Niet geregistreerd"),
            //        new("Uitstoot deeltjes (zwaar)", value: "Niet geregistreerd"),
            //        new("Roetuitstoot", value: "Niet geregistreerd"),
            //        new("Af Fabriek Roetfilter APK", value: "Niet van toepassing"),
            //        new("CO2-uitstoot NEDC", value: "165 g/km"),
            //        new("CO2-uitstoot WLTP", value: "Niet geregistreerd"),
            //        new("Emissieklasse", value: "4"),
            //        new("Milieuklasse EG", value: "EURO 4"),
            //        new("Milieuklasse EG Goedkeuring (licht)", value: "70/220*2003/76B"),
            //        new("Milieuklasse EG Goedkeuring (zwaar)", value: "Niet geregistreerd")
            //    }
            //});
            //#endregion

            // Technisch
            #region Characteristics
            result.Data.Add(new VehicleInformationSection("Eigenschappen")
            {
                Values = new List<VehicleInformationSectionValue> {
                    new("Aantal zitplaatsen", value: data["aantal_zitplaatsen"].ToString()),
                    new("Aantal rolstoelplaatsen", value: data["aantal_rolstoelplaatsen"].ToString() == "0" ? "Niet geregistreerd" : data["aantal_rolstoelplaatsen"].ToString()),
                    new("Aantal assen", value: "2"), // Assuming this is always 2
                    new("Aantal wielen", value: data["aantal_wielen"].ToString()),
                    new("Wielbasis", value: data["wielbasis"].ToString() + " cm"),
                    new("Afstand voorzijde voertuig tot hart koppeling", value: data["afstand_voorzijde_voertuig_tot_hart_koppeling"].ToString() == "0" ? "Niet geregistreerd" : data["afstand_voorzijde_voertuig_tot_hart_koppeling"].ToString())
                }
            });
            #endregion

            #region Shafts
            result.Data.Add(new VehicleInformationSection("Assen")
            {
                Values = new List<VehicleInformationSectionValue> {
                    new("Aantal zitplaatsen", value: "5"),
                    new("Aantal rolstoelplaatsen", value: "Niet geregistreerd"),
                    new("Aantal assen", value: "2"),
                    new("Aantal wielen", value: "4"),
                    new("Wielbasis", value: "259 cm"),
                    new("Afstand voorzijde voertuig tot hart koppeling", value: "Niet geregistreerd"),
                }
            });
            #endregion
        }

        return Ok(result);
    }


    /// <summary>
    /// https://opendata.rdw.nl/resource/m9d7-ebf2.json?kenteken=87GRN6
    /// </summary>
    public async Task<JToken?> GetVehicle(string licensePlate)
    {
        licensePlate = licensePlate.Replace("-", "").ToUpper();
        var url = $"https://opendata.rdw.nl/resource/m9d7-ebf2.json?kenteken={licensePlate}";

        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("X-App-Token", "OKPXTphw9Jujrm9kFGTqrTg3x");
        request.Headers.Add("Accept", "application/json");
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JArray.Parse(json)?.First();
    }

    /// <summary>
    /// https://opendata.rdw.nl/resource/3huj-srit.json?kenteken=87GRN6
    /// </summary>
    public async Task<JArray?> GetVehicleShafts(string licensePlate)
    {
        licensePlate = licensePlate.Replace("-", "").ToUpper();
        var url = $"https://opendata.rdw.nl/resource/3huj-srit.json?kenteken={licensePlate}";

        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("X-App-Token", "OKPXTphw9Jujrm9kFGTqrTg3x");
        request.Headers.Add("Accept", "application/json");
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JArray.Parse(json);
    }

    /// <summary>
    /// https://opendata.rdw.nl/Voertuigen/Open-Data-RDW-Tellerstandoordeel-Trend-Toelichting/jqs4-4kvw
    /// </summary>
    public string GetCounterReadingsDescription(string judgement)
    {
        switch (judgement)
        {
            case "00":
                return "De geregistreerde tellerstand is steeds hoger dan de daarvoor geregistreerde tellerstand. Wij oordelen dan dat de tellerstand logisch verklaarbaar is.";
            case "01":
                return "Dit voertuig heeft een teller die niet tot 999.999 telt of de teller heeft de maximale tellerstand bereikt en is daarna weer teruggesprongen naar nul. Hierdoor komt de tellerstand op de teller mogelijk niet overeen met het aantal kilometers/mijlen dat het voertuig echt gereden heeft. Daarom geven wij geen oordeel over de reeks tellerstanden van dit voertuig.";
            case "02":
                return "In dit voertuig is de teller vervangen of gerepareerd. In dit geval is het niet duidelijk hoeveel het voertuig precies heeft gereden.Daarom geven wij geen oordeel over de reeks tellerstanden van dit voertuig.";
            case "03":
                return "De geregistreerde tellerstand is steeds hoger dan de daarvoor geregistreerde tellerstand. Wij oordelen dan dat de tellerstand logisch verklaarbaar is.";
            case "04":
                return "In de reeks tellerstanden is een tellerstand geregistreerd die lager is dan de stand daarvoor. Het kan zijn dat de teller is teruggedraaid of dat er een typfout is gemaakt. Wij baseren het oordeel 'onlogisch' alleen op metingen na 1 januari 2014. Sinds die datum zijn wij verantwoordelijk voor de registratie van tellerstanden.";
            case "05":
                return "Wij geven geen oordeel over de reeks tellerstanden van dit voertuig. Dit kan twee oorzaken hebben: dit voertuig is buiten Nederland geregistreerd geweest, of aan dit voertuig is iets veranderd waardoor het een ander kenteken heeft gekregen.";
            case "06":
                return "Voor dit voertuig zijn minder dan twee standen geregistreerd. Wij kunnen alleen een oordeel geven over een reeks van tellerstanden. Daarom geven wij geen oordeel over de tellerstand.";
            case "07":
                return "De tellerstand van dit voertuig heeft van de Stichting NAP, die eerder tellerstanden in Nederland registreerde, het oordeel 'onlogisch' gekregen. Het kan zijn dat de teller is teruggedraaid of dat er een typfout is gemaakt. Wij mogen het oordeel 'onlogisch' alleen geven bij metingen na 1 januari 2014. Daarom geven wij geen oordeel over de betrouwbaarheid van de gehele reeks tellerstanden van dit voertuig.";
            default://NG
                return "Niet geregistreerd.";
        }
    }


}
