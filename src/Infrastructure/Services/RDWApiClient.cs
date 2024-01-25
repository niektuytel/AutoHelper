using System.Collections.Generic;
using System.IO;
using System.Net;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages._DTOs;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoHelper.Infrastructure.Common.Extentions;
using Azure;
using GoogleApi.Entities.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AutoHelper.Infrastructure.Services;

internal partial class RDWApiClient
{
    private readonly HttpClient _httpClient;

    public RDWApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// https://opendata.rdw.nl/resource/m9d7-ebf2.json?kenteken=87GRN6
    /// </summary>
    public async Task<JToken?> GetVehicle(string licensePlate)
    {
        licensePlate = licensePlate.Replace("-", "").ToUpper();
        var url = $"https://opendata.rdw.nl/resource/m9d7-ebf2.json?kenteken={licensePlate}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("X-App-Token", "OKPXTphw9Jujrm9kFGTqrTg3x");
        request.Headers.Add("Accept", "application/json");
        var response = await _httpClient.SendAsync(request);
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

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("X-App-Token", "OKPXTphw9Jujrm9kFGTqrTg3x");
        request.Headers.Add("Accept", "application/json");
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JArray.Parse(json);
    }

    /// <summary>
    /// https://opendata.rdw.nl/resource/8ys7-d773.json?kenteken=87GRN6
    /// </summary>
    public async Task<JToken?> GetVehicleFuel(string licensePlate)
    {
        licensePlate = licensePlate.Replace("-", "").ToUpper();
        var url = $"https://opendata.rdw.nl/resource/8ys7-d773.json?kenteken={licensePlate}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("X-App-Token", "OKPXTphw9Jujrm9kFGTqrTg3x");
        request.Headers.Add("Accept", "application/json");
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JArray.Parse(json)?.First();
    }

    internal VehicleInfoSectionItem GetFiscalData(JToken data)
    {
        return new VehicleInfoSectionItem("Fiscaal")
        {
            Values = new List<List<string>> {
            new() { "Bruto BPM", $"€ {data.GetSafeValue("bruto_bpm")}" },
            new() { "Catalogusprijs", $"€ {data.GetSafeValue("catalogusprijs")}" }
        }
        };
    }

    internal VehicleInfoSectionItem GetShaftsData(JArray? shafts)
    {
        if (shafts == null)
        {
            throw new ArgumentNullException(nameof(shafts), "Shafts data is required.");
        }

        var numbers = shafts.Select((x) => x.GetSafeValue("as_nummer")).Prepend("Nr").ToList();
        var drivenShafts = shafts.Select((x) => "Niet geregistreerd").Prepend("Aangedreven as").ToList(); // do not have the value for this
        var placedCodeShafts = shafts.Select((x) => "Niet geregistreerd").Prepend("Plaatscode as").ToList(); // do not have the value for this
        var trackWidth = shafts.Select((x) => $"{x.GetSafeValue("spoorbreedte")} cm").Prepend("Spoorbreedte").ToList();
        var misconductCode = shafts.Select((x) => "Niet geregistreerd").Prepend("Weggedrag code").ToList(); // do not have the value for this
        var maxWeightTechinicalShafts = shafts.Select((x) => $"{x.GetSafeValue("technisch_toegestane_maximum_aslast")} kg").Prepend("Technisch toegestane maximum aslast").ToList();
        var maxWeightLegalShafts = shafts.Select((x) => $"{x.GetSafeValue("wettelijk_toegestane_maximum_aslast")} kg").Prepend("Wettelijk toegestane maximum aslast").ToList();

        return new VehicleInfoSectionItem("Assen")
        {
            Values = new List<List<string>>
        {
            numbers,
            drivenShafts,
            placedCodeShafts,
            trackWidth,
            misconductCode,
            maxWeightTechinicalShafts,
            maxWeightLegalShafts
        }
        };
    }

    internal VehicleInfoSectionItem GetCharacteristicsData(JToken data)
    {
        return new VehicleInfoSectionItem("Eigenschappen")
        {
            Values = new List<List<string>>
        {
            new(){"Aantal zitplaatsen", data.GetSafeValue("aantal_zitplaatsen")},
            new(){"Aantal rolstoelplaatsen", data.GetSafeValue("aantal_rolstoelplaatsen") == "0" ? "Niet geregistreerd" : data.GetSafeValue("aantal_rolstoelplaatsen")},
            new(){"Aantal assen", "2"}, // Assuming this is always 2
            new(){"Aantal wielen", data.GetSafeValue("aantal_wielen")},
            new(){"Wielbasis", $"{data.GetSafeValue("wielbasis")} cm"},
            new(){"Afstand voorzijde voertuig tot hart koppeling", data.GetSafeValue("afstand_voorzijde_voertuig_tot_hart_koppeling") == "0" ? "Niet geregistreerd" : $"{data.GetSafeValue("afstand_voorzijde_voertuig_tot_hart_koppeling")} cm"}
        }
        };
    }

    internal VehicleInfoSectionItem GetEmissions(JToken fuelInfo)
    {
        if (!fuelInfo.HasValues)
            return null;  // or you could throw an exception or handle it some other way depending on your needs.

        return new VehicleInfoSectionItem("Uitstoot")
        {
            Values = new List<List<string>>
        {
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
            new(){"Milieuklasse EG Goedkeuring (zwaar)", fuelInfo.GetSafeValue("milieuklasse_eg_goedkeuring_zwaar")}
        }
        };
    }

    internal VehicleInfoSectionItem GetEnvironmentalPerformance(JToken fuelInfo)
    {
        if (fuelInfo?.HasValues != true)
            return null;  // or you could throw an exception or handle it some other way depending on your needs.

        var fuelUsages = $"{fuelInfo.GetSafeValue("brandstofverbruik_gecombineerd")} l/100km";
        var netMaximumPower = $"{fuelInfo.GetSafeValue("nettomaximumvermogen")} kW";

        var fuelUsagesWltp = $"{fuelInfo.GetSafeValue("brandstof_verbruik_gecombineerd_wltp")} l/100km";
        var decibelstationair = $"{fuelInfo.GetSafeValue("geluidsniveau_stationair")} dB(A)";
        var idleSpeed = $"{fuelInfo.GetSafeValue("toerental_geluidsniveau")} min-1";
        var soundlevelDriving = $"{fuelInfo.GetSafeValue("geluidsniveau_rijdend")} dB(A)";

        return new VehicleInfoSectionItem("Milleuprestaties")
        {
            Values = new List<List<string>>
        {
            new(){"Brandstof", $"{fuelInfo.GetSafeValue("brandstof_omschrijving")}"},
            new(){"Brandstofverbruik NEDC", fuelUsages},
            new(){"Brandstofverbruik WLTP", fuelUsagesWltp},
            new(){"Elektrisch verbruik NEDC", fuelInfo.GetSafeValue("elektrisch_verbruik_extern_opladen_wltp")},
            new(){"Elektrisch verbruik WLTP", fuelInfo.GetSafeValue("elektrisch_verbruik_enkel_elektrisch_wltp")},
            new(){"Elektrische actieradius NEDC", fuelInfo.GetSafeValue("actie_radius_enkel_elektrisch_stad_wltp")},
            new(){"Elektrische actieradius WLTP", fuelInfo.GetSafeValue("actie_radius_enkel_elektrisch_wltp")},
            new(){"Geluidsniveau stationair", decibelstationair},
            new(){"Geluidsniveau toerental motor", idleSpeed},
            new(){"Geluidsniveau rijdend", soundlevelDriving},
            new(){"Nettomaximumvermogen", netMaximumPower},
            new(){"Nominaal continu maximumvermogen", fuelInfo.GetSafeValue("nominaal_continu_maximumvermogen")},
            new(){"Maximum vermogen 60 minuten", fuelInfo.GetSafeValue("max_vermogen_60_minuten")},
            new(){"Netto maximumvermogen elektrisch", fuelInfo.GetSafeValue("netto_max_vermogen_elektrisch")},
        }
        };
    }

    internal VehicleInfoSectionItem GetMotorData(JToken data)
    {
        return new VehicleInfoSectionItem("Moter")
        {
            Values = new List<List<string>>
        {
            new(){"Cilinderinhoud", $"{data.GetSafeValue("cilinderinhoud")} cm³"},
            new(){"Aantal cilinders", data.GetSafeValue("aantal_cilinders")},
            new(){"Type gasinstallatie", "Niet van toepassing"},
            new(){"Emissieklasse diesel", "Niet van toepassing"},
        }
        };
    }

    internal VehicleInfoSectionItem GetRecallData(JToken data)
    {
        // Adjust based on the possible values in the dataset
        string terugroepactieStatus = data.GetSafeValue("openstaande_terugroepactie_indicator") == "Nee"
            ? "Geen terugroepactie(s) geregistreerd"
            : "Er zijn openstaande terugroepactie(s)";

        return new VehicleInfoSectionItem("Terugroepacties")
        {
            Values = new List<List<string>>
        {
            new(){"Status terugroepactie(s)", terugroepactieStatus},
        }
        };
    }

    internal VehicleInfoSectionItem GetVehicleStatus(JToken data)
    {
        return new VehicleInfoSectionItem("Status van het voertuig")
        {
            Values = new List<List<string>>
        {
            // https://groups.google.com/g/voertuigen-open-data/search?q=gestolen
            //new(){"Gestolen", $"Niet geregistreerd"},  // Replace with actual data extraction logic if available
            new(){"Geëxporteerd", $"{data.GetSafeValue("export_indicator")}"}, // Assuming "export_indicator" field holds this value
            new(){"WAM verzekerd", $"{data.GetSafeValue("wam_verzekerd")}"},
            //// Not sure which field from JSON indicates "Verbod voor rijden op de weg", for demonstration, I've left it as static "Nee"
            //new(){"Verbod voor rijden op de weg", "Niet geregistreerd"},  // Replace with actual data extraction logic if available
            new(){"Tenaamstellen mogelijk", $"{data.GetSafeValue("tenaamstellen_mogelijk")}" }
        }
        };
    }

    internal VehicleInfoSectionItem GetCounterReadings(JToken data)
    {
        return new VehicleInfoSectionItem("Tellerstanden")
        {
            Values = new List<List<string>>
        {
            new() {"Jaar laatste registratie", $"{data.GetSafeValue("jaar_laatste_registratie_tellerstand")}"},
            new() {"Oordeel", $"{data.GetSafeValue("tellerstandoordeel")}"},
            new() {"Toelichting", GetVehicleCounterReadingsDescription(data.GetSafeValue("code_toelichting_tellerstandoordeel"))}
        }
        };
    }

    internal VehicleInfoSectionItem GetWeightsData(JToken data)
    {
        return new VehicleInfoSectionItem("Gewichten")
        {
            Values = new List<List<string>>
        {
            new() {"Massa rijklaar", $"{data.GetSafeValue("massa_rijklaar")} kg"},
            new() {"Massa ledig voertuig", $"{data.GetSafeValue("massa_ledig_voertuig")} kg"},
            new() {"Technische max. massa voertuig", $"{data.GetSafeValue("technische_max_massa_voertuig")} kg"},
            new() {"Toegestane max. massa voertuig", $"{data.GetSafeValue("toegestane_maximum_massa_voertuig")} kg"},
            new() {"Maximum massa samenstel", $"{data.GetSafeValue("maximum_massa_samenstelling")} kg"},
            new() {"Aanhangwagen geremd", $"{data.GetSafeValue("maximum_trekken_massa_geremd")} kg"},
            new() {"Aanhangwagen ongeremd", $"{data.GetSafeValue("maximum_massa_trekken_ongeremd")} kg"},
        }
        };
    }

    internal VehicleInfoSectionItem GetExpirationDatesAndHistory(JToken data)
    {
        return new VehicleInfoSectionItem("Vervaldata en historie")
        {
            Values = new List<List<string>>
        {
            new() {"Vervaldatum APK", data.GetSafeDateValue("vervaldatum_apk_dt")},
            new() {"Datum eerste tenaamstelling in Nederland", data.GetSafeDateValue("datum_eerste_tenaamstelling_in_nederland_dt")},
            new() {"Datum eerste toelating", data.GetSafeDateValue("datum_eerste_toelating_dt")},
            new() {"Datum inschrijving voertuig in Nederland", data.GetSafeDateValue("datum_eerste_toelating_in_nederland_dt")},
            // There is no "Registratie datum goedkeuring" in the provided JSON, so we set a default value
            //new(){"Registratie datum goedkeuring", "Niet geregistreerd"},
            new() {"Datum laatste tenaamstelling", data.GetSafeDateValue("datum_tenaamstelling_dt")},
            // There's no separate "Tijdstip laatste tenaamstelling" field in the JSON. Using the time part of "datum_tenaamstelling_dt"
            //new(){"Tijdstip laatste tenaamstelling", "Niet geregistreerd"},
        }
        };
    }

    internal VehicleInfoSectionItem GetGeneralData(JToken data)
    {
        return new VehicleInfoSectionItem("Algemeen")
        {
            Values = new List<List<string>>
        {
            new() {"Voertuigcategorie", data.GetSafeValue("europese_voertuigcategorie")},
            // new(){"Carrosserietype", $"Hatchback (AB)"}, // This is not registered
            new() {"Inrichting", data.GetSafeValue("inrichting")},
            new() {"Merk", data.GetSafeValue("merk")},
            new() {"Type", data.GetSafeValue("type")},
            new() {"Variant", data.GetSafeValue("variant")},
            new() {"Uitvoering", data.GetSafeValue("uitvoering")},
            new() {"Kleur", data.GetSafeValue("eerste_kleur")},
            new() {"Handelsbenaming", data.GetSafeValue("handelsbenaming")},
            new() {"Typegoedkeuringsnummer", data.GetSafeValue("typegoedkeuringsnummer")},
            new() {"Plaats chassisnummer", data.GetSafeValue("plaats_chassisnummer")},
            // The is no registered value for this owners amount: '4/0'
            //new(){"Aantal eigenaren privé / zakelijk", "Niet geregistreerd"},
        }
        };
    }

    /// <summary>
    /// https://opendata.rdw.nl/Voertuigen/Open-Data-RDW-Tellerstandoordeel-Trend-Toelichting/jqs4-4kvw
    /// </summary>
    public string GetVehicleCounterReadingsDescription(string judgement)
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

    /// <summary>
    /// https://opendata.rdw.nl/resource/5k74-3jha.json
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<RDWCompany>> GetAllCompanies()
    {
        var url = $"https://opendata.rdw.nl/resource/5k74-3jha.json";
        var allCompanies = new List<RDWCompany>();
        var limit = 4000;
        var offset = 0;

        do
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{url}?$limit={limit}&$offset={offset*limit}");
            request.Headers.Add("X-App-Token", "OKPXTphw9Jujrm9kFGTqrTg3x");
            request.Headers.Add("Accept", "application/json");
            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var page = JsonConvert.DeserializeObject<IEnumerable<RDWCompany>>(json) ?? new List<RDWCompany>();

                allCompanies.AddRange(page!);
                offset++;
            }
            else
            {
                break;
            }
        }
        while (allCompanies.Count == (limit * offset));

        return allCompanies;
    }

    /// <summary>
    /// https://opendata.rdw.nl/resource/nmwb-dqkz.json
    /// 
    /// All Types:
    /// - Fotograaf bemand
    /// - APK Lichte voertuigen
    /// - Bedrijfsvoorraad
    /// - Handelaarskenteken
    /// - Tenaamstellen
    /// - APK Zware voertuigen
    /// - Controleapparaten
    /// - Uitvoer
    /// - Versnelde inschrijving
    /// - Ombouwmelding Snorfiets
    /// - Export dienstverlening
    /// - Demontage
    /// - Verplichtingennemer
    /// - APK-Landbouw
    /// - Gasinstallaties
    /// - Boordcomputertaxi
    /// - Kentekenplaatfabrikant
    /// - Onderzoeksgerechtigde
    /// - Lamineerder
    /// - Fotograaf onbemand
    /// - Kentekenloket
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<RDWCompanyService>> GetAllServices(bool skipOnUnkownServiceType = true)
    {
        var url = $"https://opendata.rdw.nl/resource/nmwb-dqkz.json";
        var allServices = new List<RDWCompanyService>();
        var limit = 30000;
        var offset = 0;

        do
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{url}?$limit={limit}&$offset={offset * limit}");
            request.Headers.Add("X-App-Token", "OKPXTphw9Jujrm9kFGTqrTg3x");
            request.Headers.Add("Accept", "application/json");
            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var page = JsonConvert.DeserializeObject<IEnumerable<RDWCompanyService>>(json) ?? new List<RDWCompanyService>();
                foreach (var item in page)
                {
                    var services = CreateServiceItems(item.Erkenning);
                    if (skipOnUnkownServiceType && services?.Any() != true)
                    {
                        continue;
                    }

                    item.RelatedServiceItems = services;
                    allServices.Add(item);
                }

                offset++;
            }
            else
            {
                break;
            }
        }
        while (allServices.Count == (limit * offset));

        return allServices;
    }

    private static List<GarageLookupServiceItem> CreateServiceItems(string rdwErkenning)
    {
        var items = new List<GarageLookupServiceItem>();
        switch(rdwErkenning)
        {
            case "Demontage":
                items.Add(new GarageLookupServiceItem()
                {
                    Type = GarageServiceType.Service,
                    VehicleType = VehicleType.Any,
                    Title = "Voertuig slopen",
                    Description = "Veilig ontmantelen en recyclen van oude en beschadigde voertuigen."
                });
                break;
            case "Kentekenplaatfabrikant":
                items.Add(new GarageLookupServiceItem()
                {
                    Type = GarageServiceType.Service,
                    VehicleType = VehicleType.Any,
                    Title = "Kentekenplaat maken",
                    Description = "Vervaardiging en levering van kentekenplaten volgens wettelijke specificaties."
                });
                break;
            case "APK Lichte voertuigen":
                items.Add(new GarageLookupServiceItem()
                {
                    Type = GarageServiceType.Inspection,
                    VehicleType = VehicleType.Motorcycle,
                    Title = "APK keuring",
                    Description = "Een wettelijk verplichte keuring om de verkeersveiligheid, milieuaspecten en registratie van uw voertuig te controleren en te waarborgen.",
                    ExpectedNextDateIsRequired = true,
                    ExpectedNextOdometerReadingIsRequired = true
                });
                items.Add(new GarageLookupServiceItem()
                {
                    Type = GarageServiceType.Inspection,
                    VehicleType = VehicleType.LightCar,
                    Title = "APK keuring",
                    Description = "Een wettelijk verplichte keuring om de verkeersveiligheid, milieuaspecten en registratie van uw voertuig te controleren en te waarborgen.",
                    ExpectedNextDateIsRequired = true,
                    ExpectedNextOdometerReadingIsRequired = true
                });
                items.Add(new GarageLookupServiceItem()
                {
                    Type = GarageServiceType.Inspection,
                    VehicleType = VehicleType.Taxi,
                    Title = "APK keuring",
                    Description = "Een wettelijk verplichte keuring om de verkeersveiligheid, milieuaspecten en registratie van uw voertuig te controleren en te waarborgen.",
                    ExpectedNextDateIsRequired = true,
                    ExpectedNextOdometerReadingIsRequired = true
                });
                break;
                
            case "APK Zware voertuigen":
                items.Add(new GarageLookupServiceItem()
                {
                    Type = GarageServiceType.Inspection,
                    VehicleType = VehicleType.HeavyCar,
                    Title = "APK keuring",
                    Description = "Een wettelijk verplichte keuring om de verkeersveiligheid, milieuaspecten en registratie van uw voertuig te controleren en te waarborgen.",
                    ExpectedNextDateIsRequired = true,
                    ExpectedNextOdometerReadingIsRequired = true
                });
                items.Add(new GarageLookupServiceItem()
                {
                    Type = GarageServiceType.Inspection,
                    VehicleType = VehicleType.Bus,
                    Title = "APK keuring",
                    Description = "Een wettelijk verplichte keuring om de verkeersveiligheid, milieuaspecten en registratie van uw voertuig te controleren en te waarborgen.",
                    ExpectedNextDateIsRequired = true,
                    ExpectedNextOdometerReadingIsRequired = true
                });
                items.Add(new GarageLookupServiceItem()
                {
                    Type = GarageServiceType.Inspection,
                    VehicleType = VehicleType.Truck,
                    Title = "APK keuring",
                    Description = "Een wettelijk verplichte keuring om de verkeersveiligheid, milieuaspecten en registratie van uw voertuig te controleren en te waarborgen.",
                    ExpectedNextDateIsRequired = true,
                    ExpectedNextOdometerReadingIsRequired = true
                });
                break;

            case "APK-Landbouw":
                items.Add(new GarageLookupServiceItem()
                {
                    Type = GarageServiceType.Inspection,
                    VehicleType = VehicleType.Tractor,
                    Title = "APK keuring",
                    Description = "Een wettelijk verplichte keuring om de verkeersveiligheid, milieuaspecten en registratie van uw voertuig te controleren en te waarborgen.",
                    ExpectedNextDateIsRequired = true
                });
                break;

            case "Controleapparaten":
                items.Add(new GarageLookupServiceItem()
                {
                    Type = GarageServiceType.Service,
                    VehicleType = VehicleType.Bus,
                    Title = "Besturingsapparaat Onderhoud",
                    Description = "RDW Gecertificeerd voor onderhoud en reparatie aan voertuigbesturingsapparaten."
                });
                items.Add(new GarageLookupServiceItem()
                {
                    Type = GarageServiceType.Service,
                    VehicleType = VehicleType.Truck,
                    Title = "Besturingsapparaat Onderhoud",
                    Description = "RDW Gecertificeerd voor onderhoud en reparatie aan voertuigbesturingsapparaten."
                });
                break;

            case "Gasinstallaties":
                items.Add(new GarageLookupServiceItem()
                {
                    Type = GarageServiceType.Service,
                    VehicleType = VehicleType.Any,
                    Title = "Gas installatie onderhoud",
                    Description = "RDW Gecertificeerd voor onderhoud en reparatie van voertuig gasinstallaties."
                });
                break;

            case "Boordcomputertaxi" :
                items.Add(new GarageLookupServiceItem()
                {
                    Type = GarageServiceType.Inspection,
                    VehicleType = VehicleType.Taxi,
                    Title = "Taxi computer onderhoud",
                    Description = "RDW Gecertificeerd voor onderhoud en reparatie van gespecialiseerde computersystemen voor taxi's."
                });
                break;
        };

        return items;
    }

    /// <summary>
    /// https://opendata.rdw.nl/resource/hx2c-gt7k.json
    /// </summary>
    /// <exception cref="Exception">When issue on api http call</exception>
    internal async Task<IEnumerable<VehicleDetectedDefectDescriptionDtoItem>> GetDetectedDefectDescriptions()
    {
        var url = $"https://opendata.rdw.nl/resource/hx2c-gt7k.json";
        var request = new HttpRequestMessage(HttpMethod.Get, $"{url}");
        request.Headers.Add("X-App-Token", "OKPXTphw9Jujrm9kFGTqrTg3x");
        request.Headers.Add("Accept", "application/json");

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"RDW API returned status code {response.StatusCode}");
        }

        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<IEnumerable<VehicleDetectedDefectDescriptionDtoItem>>(json) ?? new List<VehicleDetectedDefectDescriptionDtoItem>();
    }

    /// <summary>
    /// https://opendata.rdw.nl/resource/m9d7-ebf2.json
    /// </summary>
    public async Task<VehicleBasicsDtoItem> GetBasicVehicle(string licensePlate)
    {
        licensePlate = licensePlate.Replace("-", "").ToUpper();

        // Define the categories that require MOT.
        string[] selectedFields = new string[] { "kenteken", "vervaldatum_apk_dt", "datum_tenaamstelling_dt", "datum_eerste_toelating_dt" };

        // Create a comma-separated list of selected fields.
        string selectedFieldsQuery = selectedFields != null && selectedFields.Length > 0
                                     ? $"$select={string.Join(",", selectedFields)}&"
                                     : string.Empty;

        // Construct the full URL with the $where and $select clauses.
        var url = $"https://opendata.rdw.nl/resource/m9d7-ebf2.json?$where=(kenteken=%27{licensePlate}%27)&{selectedFieldsQuery}";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("X-App-Token", "OKPXTphw9Jujrm9kFGTqrTg3x");
        request.Headers.Add("Accept", "application/json");
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var entities = JsonConvert.DeserializeObject<IEnumerable<VehicleBasicsDtoItem>>(json) ?? new List<VehicleBasicsDtoItem>();
        return entities.FirstOrDefault() ?? new VehicleBasicsDtoItem();
    }

    /// <summary>
    /// https://opendata.rdw.nl/resource/m9d7-ebf2.json
    /// </summary>
    public async Task<IEnumerable<VehicleBasicsDtoItem>> GetVehicleBasicsWithMOTRequirement(int offset, int limit)
    {
        // Define the categories that require MOT.
        string[] apkRequiredCategories = { "M1", "M2", "M3", "N1", "N2", "N3", "O1", "O2", "O3", "O4", "L5", "L7" };
        string[] selectedFields = new string[] { "kenteken", "vervaldatum_apk_dt", "datum_tenaamstelling_dt", "datum_eerste_toelating_dt" };

        // Construct the $where clause to filter by these categories.
        string whereClause = string.Join(" OR ", apkRequiredCategories.Select(cat => $"europese_voertuigcategorie='{cat}'"));

        // Encode the whereClause to ensure it is URL-safe.
        string encodedWhereClause = WebUtility.UrlEncode($"({whereClause}) AND {selectedFields[1]} IS NOT NULL AND {selectedFields[2]} IS NOT NULL");

        // Create a comma-separated list of selected fields.
        string selectedFieldsQuery = selectedFields != null && selectedFields.Length > 0
                                     ? $"$select={string.Join(",", selectedFields)}&"
                                     : string.Empty;

        // Construct the full URL with the $where and $select clauses.
        var url = $"https://opendata.rdw.nl/resource/m9d7-ebf2.json?{selectedFieldsQuery}$where={encodedWhereClause}&$limit={limit}&$offset={offset * limit}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("X-App-Token", "OKPXTphw9Jujrm9kFGTqrTg3x");
        request.Headers.Add("Accept", "application/json");
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<IEnumerable<VehicleBasicsDtoItem>>(json) ?? new List<VehicleBasicsDtoItem>();
    }

    /// <summary>
    /// https://opendata.rdw.nl/resource/m9d7-ebf2.json
    /// </summary>
    internal async Task<int> GetVehicleBasicsWithMOTRequirementCount()
    {
        // Define the categories that require MOT.
        string[] apkRequiredCategories = { "M1", "M2", "M3", "N1", "N2", "N3", "O1", "O2", "O3", "O4", "L5", "L7" };
        string[] selectedFields = new string[] { "kenteken", "vervaldatum_apk_dt", "datum_tenaamstelling_dt", "datum_eerste_toelating_dt" };

        // Construct the $where clause to filter by these categories.
        string whereClause = string.Join(" OR ", apkRequiredCategories.Select(cat => $"europese_voertuigcategorie='{cat}'"));

        // Encode the whereClause to ensure it is URL-safe.
        string encodedWhereClause = WebUtility.UrlEncode($"({whereClause}) AND {selectedFields[1]} IS NOT NULL AND {selectedFields[2]} IS NOT NULL");


        // Construct the full URL with the $where clause.
        var url = $"https://opendata.rdw.nl/resource/m9d7-ebf2.json?$where={encodedWhereClause}&$select=count(*)";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("X-App-Token", "OKPXTphw9Jujrm9kFGTqrTg3x");
        request.Headers.Add("Accept", "application/json");
        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"RDW API returned status code {response.StatusCode}");
        }

        var json = await response.Content.ReadAsStringAsync();
        var jsonArray = JArray.Parse(json);

        // Check if the JSON array has at least one element
        if (jsonArray.Count > 0)
        {
            // Get the first element of the JSON array
            var firstElement = jsonArray[0];

            // Retrieve the count value
            var countValue = firstElement.Value<int>("count");

            return countValue;
        }
        else
        {
            throw new Exception("No data returned from RDW API");
        }
    }

    /// <summary>
    /// https://opendata.rdw.nl/resource/a34c-vvps.json?$where=(kenteken='RV231P'%20OR%20kenteken='87GRN6')
    /// </summary>
    internal async Task<IEnumerable<VehicleDetectedDefectDtoItem>> GetVehicleDetectedDefects(List<string> licensePlates, int offset, int limit)
    {
        var url = $"https://opendata.rdw.nl/resource/a34c-vvps.json";
        var query = string.Join(" OR ", licensePlates.Select(licensePlate => $"kenteken='{licensePlate}'"));
        var request = new HttpRequestMessage(HttpMethod.Get, $"{url}?$where=({query})&$limit={limit}&$offset={offset * limit}");
        request.Headers.Add("X-App-Token", "OKPXTphw9Jujrm9kFGTqrTg3x");
        request.Headers.Add("Accept", "application/json");

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"RDW API returned status code {response.StatusCode}");
        }

        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<IEnumerable<VehicleDetectedDefectDtoItem>>(json) ?? new List<VehicleDetectedDefectDtoItem>();
    }

    /// <summary>
    /// https://opendata.rdw.nl/resource/sgfe-77wx.json?$where=(kenteken='RV231P'%20OR%20kenteken='87GRN6')
    /// </summary>
    /// <exception cref="Exception">When issue on api http call</exception>
    internal async Task<IEnumerable<VehicleInspectionNotificationDtoItem>> GetVehicleInspectionNotifications(List<string> licensePlates, int offset, int limit)
    {
        var url = $"https://opendata.rdw.nl/resource/sgfe-77wx.json";
        var query = string.Join(" OR ", licensePlates.Select(licensePlate => $"kenteken='{licensePlate}'"));
        var request = new HttpRequestMessage(HttpMethod.Get, $"{url}?$where=({query})&$limit={limit}&$offset={offset * limit}");
        request.Headers.Add("X-App-Token", "OKPXTphw9Jujrm9kFGTqrTg3x");
        request.Headers.Add("Accept", "application/json");

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"RDW API returned status code {response.StatusCode}");
        }

        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<IEnumerable<VehicleInspectionNotificationDtoItem>>(json) ?? new List<VehicleInspectionNotificationDtoItem>();
    }

    /// <summary>
    /// https://opendata.rdw.nl/resource/5k74-3jha.json
    /// </summary>
    internal async Task<IEnumerable<RDWCompany>> GetAllCompanies(int offset, int limit, bool includeFiltering = true)
    {
        var url = $"https://opendata.rdw.nl/resource/5k74-3jha.json?$limit={limit}&$offset={offset * limit}";
        if (includeFiltering)
        {
            // Additional filters
            string additionalFilters = "volgnummer IS NOT NULL AND naam_bedrijf IS NOT NULL AND plaats IS NOT NULL AND straat IS NOT NULL";
            string encodedWhereClause = WebUtility.UrlEncode(additionalFilters);
            url += $"&$where={encodedWhereClause}";
        }

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("X-App-Token", "OKPXTphw9Jujrm9kFGTqrTg3x");
        request.Headers.Add("Accept", "application/json");
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<IEnumerable<RDWCompany>>(json) ?? new List<RDWCompany>();
    }

    /// <summary>
    /// https://opendata.rdw.nl/resource/5k74-3jha.json
    /// </summary>
    internal async Task<int> GetAllCompaniesCount(bool includeFiltering = true)
    {
        var url = $"https://opendata.rdw.nl/resource/5k74-3jha.json?$select=count(*)";
        if (includeFiltering)
        {
            // Additional filters
            string additionalFilters = "volgnummer IS NOT NULL AND naam_bedrijf IS NOT NULL AND plaats IS NOT NULL AND straat IS NOT NULL";
            string encodedWhereClause = WebUtility.UrlEncode(additionalFilters);
            url += $"&$where={encodedWhereClause}";
        }

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("X-App-Token", "OKPXTphw9Jujrm9kFGTqrTg3x");
        request.Headers.Add("Accept", "application/json");
        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"RDW API returned status code {response.StatusCode}");
        }

        var json = await response.Content.ReadAsStringAsync();
        var jsonArray = JArray.Parse(json);

        // Check if the JSON array has at least one element
        if (jsonArray.Count > 0)
        {
            // Get the first element of the JSON array
            var firstElement = jsonArray[0];

            // Retrieve the count value
            var countValue = firstElement.Value<int>("count");

            return countValue;
        }
        else
        {
            throw new Exception("No data returned from RDW API");
        }
    }

}