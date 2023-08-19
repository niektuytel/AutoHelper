using System.Diagnostics.Metrics;
using AutoHelper.Application.TodoLists.Commands.CreateTodoList;
using AutoHelper.Application.TodoLists.Commands.DeleteTodoList;
using AutoHelper.Application.TodoLists.Commands.UpdateTodoList;
using AutoHelper.Application.TodoLists.Queries.ExportTodos;
using AutoHelper.Application.TodoLists.Queries.GetTodos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WebUI.Extensions;
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
                Values = new List<List<string>> {
                    new(){"Voertuigcategorie", data.GetSafeValue("europese_voertuigcategorie")},
                    // new(){"Carrosserietype", $"Hatchback (AB)"},// This is not registered
                    new(){"Inrichting", data.GetSafeValue("inrichting")},
                    new(){"Merk", data.GetSafeValue("merk")},
                    new(){"Type", data.GetSafeValue("type")},
                    new(){"Variant", data.GetSafeValue("variant")},
                    new(){"Uitvoering", data.GetSafeValue("uitvoering")},
                    new(){"Kleur", data.GetSafeValue("eerste_kleur")},
                    new(){"Handelsbenaming", data.GetSafeValue("handelsbenaming")},
                    new(){"Typegoedkeuringsnummer", data.GetSafeValue("typegoedkeuringsnummer")},
                    new(){"Plaats chassisnummer", data.GetSafeValue("plaats_chassisnummer")},
                    // The is no registered value for this owners amount: '4/0'
                    //new(){"Aantal eigenaren privé / zakelijk", "Niet geregistreerd"},
                }
            });
            #endregion

            #region Expiration dates and history
            result.Data.Add(new VehicleInformationSection("Vervaldata en historie")
            {
                Values = new List<List<string>> {
                    new(){"Vervaldatum APK", data.GetSafeDateValue("vervaldatum_apk_dt")},
                    new(){"Datum eerste tenaamstelling in Nederland", data.GetSafeDateValue("datum_eerste_tenaamstelling_in_nederland_dt")},
                    new(){"Datum eerste toelating", data.GetSafeDateValue("datum_eerste_toelating_dt")},
                    new(){"Datum inschrijving voertuig in Nederland", data.GetSafeDateValue("datum_eerste_toelating_in_nederland_dt")},
                    // There is no "Registratie datum goedkeuring" in the provided JSON, so we set a default value
                    //new(){"Registratie datum goedkeuring", "Niet geregistreerd"},
                    new(){"Datum laatste tenaamstelling", data.GetSafeDateValue("datum_tenaamstelling_dt")},
                    // There's no separate "Tijdstip laatste tenaamstelling" field in the JSON. Using the time part of "datum_tenaamstelling_dt"
                    //new(){"Tijdstip laatste tenaamstelling", "Niet geregistreerd"},
                }
            });
            #endregion

            #region Weights
            result.Data.Add(new VehicleInformationSection("Gewichten")
            {
                Values = new List<List<string>> {
                    new(){"Massa rijklaar", $"{data.GetSafeValue("massa_rijklaar")} kg"},
                    new(){"Massa ledig voertuig", $"{data.GetSafeValue("massa_ledig_voertuig")} kg"},
                    new(){"Technische max. massa voertuig", $"{data.GetSafeValue("technische_max_massa_voertuig")} kg"},
                    new(){"Toegestane max. massa voertuig", $"{data.GetSafeValue("toegestane_maximum_massa_voertuig")} kg"},
                    new(){"Maximum massa samenstel", $"{data.GetSafeValue("maximum_massa_samenstelling")} kg"},
                    new(){"Aanhangwagen geremd", $"{data.GetSafeValue("maximum_trekken_massa_geremd")} kg"},
                    new(){"Aanhangwagen ongeremd", $"{data.GetSafeValue("maximum_massa_trekken_ongeremd")} kg"},
                }
            });
            #endregion

            #region Counter readings
            result.Data.Add(new VehicleInformationSection("Tellerstanden")
            {
                Values = new List<List<string>> {
                    new(){"Jaar laatste registratie", $"{data.GetSafeValue("jaar_laatste_registratie_tellerstand")}"},
                    new(){"Oordeel", $"{data.GetSafeValue("tellerstandoordeel")}"},
                    new(){"Toelichting", GetCounterReadingsDescription(data.GetSafeValue("code_toelichting_tellerstandoordeel")) }
                }
            });
            #endregion

            #region Status of the vehicle
            result.Data.Add(new VehicleInformationSection("Status van het voertuig")
            {
                Values = new List<List<string>> {
                    // https://groups.google.com/g/voertuigen-open-data/search?q=gestolen
                    //new(){"Gestolen", $"Niet geregistreerd"},  // Replace with actual data extraction logic if available
                    new(){"Geëxporteerd", $"{data.GetSafeValue("export_indicator")}"}, // Assuming "export_indicator" field holds this value
                    new(){"WAM verzekerd", $"{data.GetSafeValue("wam_verzekerd")}"},
        
                    //// Not sure which field from JSON indicates "Verbod voor rijden op de weg", for demonstration, I've left it as static "Nee"
                    //new(){"Verbod voor rijden op de weg", "Niet geregistreerd"},  // Replace with actual data extraction logic if available
        
                    new(){"Tenaamstellen mogelijk", $"{data.GetSafeValue("tenaamstellen_mogelijk")}" }
                }
            });
            #endregion

            #region Recall
            // Adjust based on the possible values in the dataset
            string terugroepactieStatus = data.GetSafeValue("openstaande_terugroepactie_indicator") == "Nee"
                ? "Geen terugroepactie(s) geregistreerd"
                : "Er zijn openstaande terugroepactie(s)"; 

            result.Data.Add(new VehicleInformationSection("Terugroepacties")
            {
                Values = new List<List<string>> {
                    new(){"Status terugroepactie(s)", terugroepactieStatus},
                }
            });
            #endregion

            // Moter & milleu
            #region Moter
            result.Data.Add(new VehicleInformationSection("Moter")
            {
                Values = new List<List<string>> {
                    new(){"Cilinderinhoud", $"{data.GetSafeValue("cilinderinhoud")} cm³"},
                    new(){"Aantal cilinders", data.GetSafeValue("aantal_cilinders")},
                    new(){"Type gasinstallatie", "Niet van toepassing"},
                    new(){"Emissieklasse diesel", "Niet van toepassing"},
                }
            });
            #endregion

            var fuelInfo = await GetVehicleFuel(licensePlate);

            #region Environmental performance
            var fuelUsages = $"{fuelInfo!.GetSafeValue("brandstofverbruik_gecombineerd")} l/100km";
            var netMaximumPower = $"{fuelInfo!.GetSafeValue("nettomaximumvermogen")} kW";
            if (fuelInfo?.HasValues == true)
            { 
                var fuelUsagesWltp = $"{fuelInfo!.GetSafeValue("brandstof_verbruik_gecombineerd_wltp")} l/100km";
                var decibelstationair = $"{fuelInfo.GetSafeValue("geluidsniveau_stationair")} dB(A)";
                var idleSpeed = $"{fuelInfo.GetSafeValue("toerental_geluidsniveau")} min-1";
                var soundlevelDriving = $"{fuelInfo.GetSafeValue("geluidsniveau_rijdend")} dB(A)";

                result.Data.Add(new VehicleInformationSection("Milleuprestaties")
                {
                    Values = new List<List<string>> {
                        new(){"Brandstof", $"{fuelInfo.GetSafeValue("brandstof_omschrijving")}"},
                        new(){"Brandstofverbruik NEDC", fuelUsages},
                        new(){"Brandstofverbruik WLTP", fuelUsagesWltp},
                        new(){"Elektrisch verbruik NEDC", fuelInfo.GetSafeValue("elektrisch_verbruik_extern_opladen_wltp") },
                        new(){"Elektrisch verbruik WLTP", fuelInfo.GetSafeValue("elektrisch_verbruik_enkel_elektrisch_wltp") },
                        new(){"Elektrische actieradius NEDC", fuelInfo.GetSafeValue("actie_radius_enkel_elektrisch_stad_wltp") },
                        new(){"Elektrische actieradius WLTP", fuelInfo.GetSafeValue("actie_radius_enkel_elektrisch_wltp") },
                        new(){"Geluidsniveau stationair", decibelstationair},
                        new(){"Geluidsniveau toerental motor", idleSpeed },
                        new(){"Geluidsniveau rijdend", soundlevelDriving },
                        new(){"Nettomaximumvermogen", netMaximumPower},
                        new(){"Nominaal continu maximumvermogen", fuelInfo.GetSafeValue("nominaal_continu_maximumvermogen")},
                        new(){"Maximum vermogen 60 minuten", fuelInfo.GetSafeValue("max_vermogen_60_minuten")},
                        new(){"Netto maximumvermogen elektrisch", fuelInfo.GetSafeValue("netto_max_vermogen_elektrisch")},
                    }
                });
            }
            #endregion

            #region Emissions
            if (fuelInfo.HasValues)
            {
                result.Data.Add(new VehicleInformationSection("Uitstoot")
                {
                    Values = new List<List<string>> {
                        new(){"Brandstof", fuelInfo.GetSafeValue("brandstof_omschrijving")},
                        new(){"Uitstoot deeltjes (licht)", fuelInfo.GetSafeValue("uitstoot_deeltjes_licht")},
                        new(){"Uitstoot deeltjes (zwaar)", fuelInfo.GetSafeValue("uitstoot_deeltjes_zwaar")},
                        new(){"Roetuitstoot", fuelInfo.GetSafeValue("roetuitstoot")},
                        new(){"Af Fabriek Roetfilter APK", "Niet van toepassing"},
                        new(){"CO2-uitstoot NEDC", $"{fuelInfo.GetSafeValue("co2_uitstoot_gecombineerd")} g/km"},
                        new(){"CO2-uitstoot WLTP", $"{fuelInfo.GetSafeValue("emis_co2_gewogen_gecombineerd_wltp")} g/km" },
                        new(){"Emissieklasse", fuelInfo.GetSafeValue("emissiecode_omschrijving")},
                        new(){"Milieuklasse EG", fuelInfo.GetSafeValue("uitlaatemissieniveau")},
                        new(){"Milieuklasse EG Goedkeuring (licht)", fuelInfo.GetSafeValue("milieuklasse_eg_goedkeuring_licht")},
                        new(){"Milieuklasse EG Goedkeuring (zwaar)", fuelInfo.GetSafeValue("milieuklasse_eg_goedkeuring_zwaar") }
                    }
                });
            }
            #endregion

            // Technisch
            #region Characteristics
            result.Data.Add(new VehicleInformationSection("Eigenschappen")
            {
                Values = new List<List<string>> {
                    new(){"Aantal zitplaatsen", data.GetSafeValue("aantal_zitplaatsen") },
                    new(){"Aantal rolstoelplaatsen", data.GetSafeValue("aantal_rolstoelplaatsen") == "0" ? "Niet geregistreerd" : data.GetSafeValue("aantal_rolstoelplaatsen")},
                    new(){"Aantal assen", "2"}, // Assuming this is always 2
                    new(){"Aantal wielen", data.GetSafeValue("aantal_wielen")},
                    new(){"Wielbasis", data.GetSafeValue("wielbasis") + " cm"},
                    new(){"Afstand voorzijde voertuig tot hart koppeling", data.GetSafeValue("afstand_voorzijde_voertuig_tot_hart_koppeling") == "0" ? "Niet geregistreerd" : data.GetSafeValue("afstand_voorzijde_voertuig_tot_hart_koppeling") }
                }
            });
            #endregion

            #region Shafts
            var shafts = await GetVehicleShafts(licensePlate);
            var numbers = shafts!.Select((x) => x.GetSafeValue("as_nummer")).Prepend("Nr").ToList();
            var drivenShafts = shafts!.Select((x) => "Niet geregistreerd").Prepend("Aangedreven as").ToList();// do not have the value for this
            var placedCodeShafts = shafts!.Select((x) => "Niet geregistreerd").Prepend("Plaatscode as").ToList();// do not have the value for this
            var trackWidth = shafts!.Select((x) => $"{x.GetSafeValue("spoorbreedte")} cm").Prepend("Spoorbreedte").ToList();
            var misconductCode = shafts!.Select((x) => "Niet geregistreerd").Prepend("Weggedrag code").ToList();// do not have the value for this
            var maxWeightTechinicalShafts = shafts!.Select((x) => $"{x.GetSafeValue("technisch_toegestane_maximum_aslast")} kg").Prepend("Technisch toegestane maximum aslast").ToList();
            var maxWeightLegalShafts = shafts!.Select((x) => $"{x.GetSafeValue("wettelijk_toegestane_maximum_aslast")} kg").Prepend("Wettelijk toegestane maximum aslast").ToList();

            result.Data.Add(new VehicleInformationSection("Assen")
            {
                Values = new List<List<string>> {
                    numbers,
                    drivenShafts,
                    placedCodeShafts,
                    trackWidth,
                    misconductCode,
                    maxWeightTechinicalShafts,
                    maxWeightLegalShafts,
                }
            });
            #endregion

            // Fiscal
            #region Fiscal
            result.Data.Add(new VehicleInformationSection("Fiscaal")
            {
                Values = new List<List<string>> {
                    new(){ "Bruto BPM", $"€ {data.GetSafeValue("bruto_bpm")}" },
                    new(){ "Catalogusprijs", $"€ {data.GetSafeValue("catalogusprijs")}" }
                }
            });
            #endregion


            // Card Info
            var amount = fuelInfo!.GetSafeDecimalValue("brandstofverbruik_gecombineerd");
            var fuelUsagesDecription = amount != 0
                ? $"{(100M / amount).ToString("F2")}km op 1 liter {fuelInfo.GetSafeValue("brandstof_omschrijving")}"
                : "Niet geregistreerd";

            result.CardInfo = new()
            {
                new(){ "Merk", $"{data.GetSafeValue("merk")} ({data.GetSafeValue("handelsbenaming")})" },
                new(){ "Verbruik", fuelUsagesDecription },
                new(){ "Vermogen", netMaximumPower },
                new(){ "Vervaldatum APK", data.GetSafeDateValue("vervaldatum_apk_dt") },
                new(){ "Kilometer stand", $"{data.GetSafeValue("tellerstandoordeel")}" },

            };

        }

        return Ok(result);
    }

    /// <summary>
    /// https://opendata.rdw.nl/resource/m9d7-ebf2.json?kenteken=87GRN6
    /// </summary>
    private async Task<JToken?> GetVehicle(string licensePlate)
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
    private async Task<JArray?> GetVehicleShafts(string licensePlate)
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
    /// https://opendata.rdw.nl/resource/8ys7-d773.json?kenteken=87GRN6
    /// </summary>
    private async Task<JToken?> GetVehicleFuel(string licensePlate)
    {
        licensePlate = licensePlate.Replace("-", "").ToUpper();
        var url = $"https://opendata.rdw.nl/resource/8ys7-d773.json?kenteken={licensePlate}";

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
    /// https://opendata.rdw.nl/Voertuigen/Open-Data-RDW-Tellerstandoordeel-Trend-Toelichting/jqs4-4kvw
    /// </summary>
    private string GetCounterReadingsDescription(string judgement)
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
