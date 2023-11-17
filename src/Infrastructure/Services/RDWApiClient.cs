﻿using System.Collections.Generic;
using System.IO;
using System.Net;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Queries.GetVehicleBriefInfo;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Infrastructure.Common.Extentions;
using AutoHelper.Infrastructure.Common.Models;
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
    /// https://opendata.rdw.nl/resource/vkij-7mwc.json?kenteken=87GRN6
    /// </summary>
    public async Task<bool> VehicleExist(string licensePlate)
    {
        licensePlate = licensePlate.Replace("-", "").ToUpper();
        var url = $"https://opendata.rdw.nl/resource/vkij-7mwc.json?kenteken={licensePlate}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("X-App-Token", "OKPXTphw9Jujrm9kFGTqrTg3x");
        request.Headers.Add("Accept", "application/json");
        var response = await _httpClient.SendAsync(request);
        return response.IsSuccessStatusCode;
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
    public async Task<IEnumerable<RDWKnownCompany>> GetKnownCompanies()
    {
        var url = $"https://opendata.rdw.nl/resource/5k74-3jha.json";
        var allCompanies = new List<RDWKnownCompany>();
        var limit = 2000;
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
                var page = JsonConvert.DeserializeObject<IEnumerable<RDWKnownCompany>>(json) ?? new List<RDWKnownCompany>();

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
    public async Task<IEnumerable<RDWKnownService>> GetKnownServices()
    {
        var url = $"https://opendata.rdw.nl/resource/nmwb-dqkz.json";
        var allServices = new List<RDWKnownService>();
        var limit = 5000;
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
                var page = JsonConvert.DeserializeObject<IEnumerable<RDWKnownService>>(json) ?? new List<RDWKnownService>();
                foreach (var item in page)
                {
                    item.ServiceType = GetKnownServiceType(item.Erkenning);
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

    private static GarageServiceType GetKnownServiceType(string rdwErkenning)
    {
        return rdwErkenning switch
        {
            "Bedrijfsvoorraad" => GarageServiceType.CompanyStockService,
            "Tenaamstellen" => GarageServiceType.RegistrationService,
            "Versnelde inschrijving" => GarageServiceType.AcceleratedRegistrationService,
            "APK Lichte voertuigen" => GarageServiceType.MOTServiceLightVehicle,
            "APK Zware voertuigen" => GarageServiceType.MOTServiceHeavyVehicle,
            "APK-Landbouw" => GarageServiceType.MOTServiceAgriculture,
            "Controleapparaten" => GarageServiceType.ControlDeviceService,
            "Gasinstallaties" => GarageServiceType.GasInstallationService,
            "Ombouwmelding Snorfiets" => GarageServiceType.MopedConversionService,
            "Demontage" => GarageServiceType.DismantlingService,
            "Boordcomputertaxi" => GarageServiceType.TaxiComputerService,
            "Kentekenplaatfabrikant" => GarageServiceType.LicensePlateManufactureService,
            _ => GarageServiceType.Other,
        };
    }


    /// <summary>
    /// https://opendata.rdw.nl/resource/hx2c-gt7k.json
    /// </summary>
    /// <exception cref="Exception">When issue on api http call</exception>
    internal async Task<IEnumerable<RDWDetectedDefectDescription>> GetDetectedDefectDescriptions()
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
        return JsonConvert.DeserializeObject<IEnumerable<RDWDetectedDefectDescription>>(json) ?? new List<RDWDetectedDefectDescription>();
    }

    /// <summary>
    /// https://opendata.rdw.nl/resource/m9d7-ebf2.json
    /// </summary>
    public async Task<IEnumerable<RDWVehicle>> GetVehicles(int offset, int limit)
    {
        var url = $"https://opendata.rdw.nl/resource/m9d7-ebf2.json";

        var request = new HttpRequestMessage(HttpMethod.Get, $"{url}?$limit={limit}&$offset={offset * limit}");
        request.Headers.Add("X-App-Token", "OKPXTphw9Jujrm9kFGTqrTg3x");
        request.Headers.Add("Accept", "application/json");
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<IEnumerable<RDWVehicle>>(json) ?? new List<RDWVehicle>();
    }

    /// <summary>
    /// https://opendata.rdw.nl/resource/m9d7-ebf2.json
    /// </summary>
    public async Task<IEnumerable<RDWVehicleBasics>> GetVehicleBasicsWithMOTRequirement(int offset, int limit)
    {
        // Define the categories that require MOT.
        string[] apkRequiredCategories = { "M1", "M2", "M3", "N1", "N2", "N3", "O1", "O2", "O3", "O4", "L5", "L7" };
        string[] selectedFields = new string[] { "kenteken", "vervaldatum_apk_dt", "datum_tenaamstelling_dt" };

        // Construct the $where clause to filter by these categories.
        string whereClause = string.Join(" OR ", apkRequiredCategories.Select(cat => $"europese_voertuigcategorie='{cat}'")) 
            + $" AND {selectedFields[1]} IS NOT NULL"
            + $" AND {selectedFields[2]} IS NOT NULL"
        ;

        // Encode the whereClause to ensure it is URL-safe.
        string encodedWhereClause = WebUtility.UrlEncode(whereClause);

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
        return JsonConvert.DeserializeObject<IEnumerable<RDWVehicleBasics>>(json) ?? new List<RDWVehicleBasics>();
    }

    /// <summary>
    /// https://opendata.rdw.nl/resource/m9d7-ebf2.json
    /// </summary>
    internal async Task<int> GetVehicleBasicsWithMOTRequirementCount()
    {
        // Define the categories that require MOT.
        string[] apkRequiredCategories = { "M1", "M2", "M3", "N1", "N2", "N3", "O1", "O2", "O3", "O4", "L5", "L7" };
        string[] selectedFields = new string[] { "kenteken", "vervaldatum_apk_dt", "datum_tenaamstelling_dt" };

        // Construct the $where clause to filter by these categories.
        string whereClause = string.Join(" OR ", apkRequiredCategories.Select(cat => $"europese_voertuigcategorie='{cat}'"))
            + $" AND {selectedFields[1]} IS NOT NULL"
            + $" AND {selectedFields[2]} IS NOT NULL"
        ;

        // Encode the whereClause to ensure it is URL-safe.
        string encodedWhereClause = WebUtility.UrlEncode(whereClause);

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
    /// https://opendata.rdw.nl/resource/a34c-vvps.json
    /// </summary>
    /// <exception cref="Exception">When issue on api http call</exception>
    internal async Task<IEnumerable<RDWVehicleDetectedDefect>> GetVehicleDetectedDefects(string licensePlate)
    {
        var url = $"https://opendata.rdw.nl/resource/a34c-vvps.json";
        var request = new HttpRequestMessage(HttpMethod.Get, $"{url}?kenteken={licensePlate}");
        request.Headers.Add("X-App-Token", "OKPXTphw9Jujrm9kFGTqrTg3x");
        request.Headers.Add("Accept", "application/json");

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"RDW API returned status code {response.StatusCode}");
        }

        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<IEnumerable<RDWVehicleDetectedDefect>>(json) ?? new List<RDWVehicleDetectedDefect>();
    }

    /// <summary>
    /// https://opendata.rdw.nl/resource/sgfe-77wx.json
    /// </summary>
    /// <exception cref="Exception">When issue on api http call</exception>
    internal async Task<IEnumerable<RDWvehicleInspectionNotification>> GetVehicleInspectionNotifications(string licensePlate)
    {
        var url = $"https://opendata.rdw.nl/resource/sgfe-77wx.json";
        var request = new HttpRequestMessage(HttpMethod.Get, $"{url}?kenteken={licensePlate}");
        request.Headers.Add("X-App-Token", "OKPXTphw9Jujrm9kFGTqrTg3x");
        request.Headers.Add("Accept", "application/json");

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"RDW API returned status code {response.StatusCode}");
        }

        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<IEnumerable<RDWvehicleInspectionNotification>>(json) ?? new List<RDWvehicleInspectionNotification>();
    }

    /// <summary>
    /// https://www.rdw.nl/particulier/paginas/voertuigcategorieen
    /// </summary>
    internal bool MOTIsRequired(string europeanVehicleCategory)
    {
        // Een lijst van APK-plichtige voertuigcategorieën
        var apkRequiredCategories = new HashSet<string> { "M1", "M2", "M3", "N1", "N2", "N3", "O1", "O2", "O3", "O4", "L5", "L7" };
        return apkRequiredCategories.Contains(europeanVehicleCategory);
    }

    /// <summary>
    /// https://opendata.rdw.nl/resource/a34c-vvps.json?$where=(kenteken='RV231P'%20OR%20kenteken='87GRN6')
    /// </summary>
    internal async Task<IEnumerable<RDWVehicleDetectedDefect>> GetVehicleDetectedDefects(List<string> licensePlates, int offset, int limit)
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
        return JsonConvert.DeserializeObject<IEnumerable<RDWVehicleDetectedDefect>>(json) ?? new List<RDWVehicleDetectedDefect>();
    }

    /// <summary>
    /// https://opendata.rdw.nl/resource/sgfe-77wx.json?$where=(kenteken='RV231P'%20OR%20kenteken='87GRN6')
    /// </summary>
    /// <exception cref="Exception">When issue on api http call</exception>
    internal async Task<IEnumerable<RDWvehicleInspectionNotification>> GetVehicleInspectionNotifications(List<string> licensePlates, int offset, int limit)
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
        return JsonConvert.DeserializeObject<IEnumerable<RDWvehicleInspectionNotification>>(json) ?? new List<RDWvehicleInspectionNotification>();
    }

}