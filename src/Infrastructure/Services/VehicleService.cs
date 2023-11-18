using System.Collections.Generic;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Queries.GetVehicleBriefInfo;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using AutoHelper.Application.Vehicles.Queries.GetVehicleSpecs;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoHelper.Infrastructure.Common.Extentions;
using AutoHelper.Infrastructure.Common.Models;
using Azure;
using Azure.Core;
using Force.DeepCloner;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AutoHelper.Infrastructure.Services;

internal class VehicleService : IVehicleService
{
    private readonly RDWApiClient _rdwService;

    public VehicleService(RDWApiClient rdwService)
    {
        _rdwService = rdwService;
    }

    public Task<IEnumerable<string>> GetAllLicensePlatesAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<VehicleBriefDtoItem?> GetVehicleByLicensePlateAsync(string licensePlate)
    {
        var data = await _rdwService.GetVehicle(licensePlate);
        if (data?.HasValues != true)
        {
            throw new NotFoundException("Vehicle data not found.");
        }

        var from = data.GetSafeDateYearValue("datum_eerste_toelating_dt");
        var fromText = from != 0 ? $" uit {from}" : string.Empty;
        var brandText = $"{data.GetSafeValue("merk")} ({data.GetSafeValue("handelsbenaming")}){fromText}";
        var mileage = data.GetSafeValue("tellerstandoordeel");
        var response = new VehicleBriefDtoItem
        {
            LicensePlate = licensePlate,
            Brand = brandText,
            Mileage = mileage,
        };

        if(DateTime.TryParse(data.GetSafeDateValue("vervaldatum_apk_dt"), out var motExpiry))
        {
            response.DateOfMOTExpiry = motExpiry;
        }

        if (DateTime.TryParse(data.GetSafeDateValue("datum_tenaamstelling_dt"), out var dateOfAscription))
        {
            response.DateOfAscription = dateOfAscription;
        }

        var fuelInfo = await _rdwService.GetVehicleFuel(licensePlate);
        if (fuelInfo?.HasValues == true)
        {
            var amount = fuelInfo.GetSafeDecimalValue("brandstofverbruik_gecombineerd");
            var consumptionText = amount != 0
                ? $"{100M / (amount * 100M):F2}KM op 1 liter {fuelInfo.GetSafeValue("brandstof_omschrijving").ToLower()}"
                : "Niet geregistreerd";
            
            response.Consumption = consumptionText;
        }

        return response;
    }

    public async Task<VehicleSpecificationsDto> GetSpecificationsByLicensePlateAsync(string licensePlate)
    {
        var response = new VehicleSpecificationsDto();
        var data = await _rdwService.GetVehicle(licensePlate);

        if (!data.HasValues)
        {
            throw new NotFoundException("Vehicle data not found.");
        }

        response.Data.Add(GetGeneralData(data));
        response.Data.Add(GetExpirationDatesAndHistory(data));
        response.Data.Add(GetWeightsData(data));
        response.Data.Add(GetCounterReadings(data));
        response.Data.Add(GetVehicleStatus(data));
        response.Data.Add(GetRecallData(data));
        response.Data.Add(GetMotorData(data));

        var fuelInfo = await _rdwService.GetVehicleFuel(licensePlate);
        if (fuelInfo.HasValues)
        {
            response.Data.Add(GetEnvironmentalPerformance(fuelInfo));
            response.Data.Add(GetEmissions(fuelInfo));
        }

        response.Data.Add(GetCharacteristicsData(data));

        var shafts = await _rdwService.GetVehicleShafts(licensePlate);
        response.Data.Add(GetShaftsData(shafts));
        response.Data.Add(GetFiscalData(data));
        return response;
    }

    // TODO: Need better investigation
    // can add more cases based on other vehicle kinds present in the RDW data.
    public async Task<VehicleType> GetVehicleTypeByLicensePlateAsync(string licensePlate)
    {
        var data = await _rdwService.GetVehicle(licensePlate);

        // Check "voertuigsoort" field for various types
        var vehicleKind = data?["voertuigsoort"]?.ToString();

        // Check weight for HeavyCar
        if (int.TryParse(data?["technische_max_massa_voertuig"]?.ToString(), out int weight) && weight > 3500)
        {
            return VehicleType.HeavyCar;
        }

        switch (vehicleKind)
        {
            case "Personenauto":
                return VehicleType.LightCar;
            case "Driewielig motorrijtuig":
                return VehicleType.Motorcycle;
            case "Land- of bosbouwtrekker":
                return VehicleType.Tractor;
            case "Land- of bosb aanhw of getr uitr stuk":
                return VehicleType.Tractor;

            default:
                break;
        }

        // Check Taxi
        if (data?["taxi_indicator"]?.ToString() == "Ja")
        {
            return VehicleType.Taxi;
        }


        // If no matches, return Other
        return VehicleType.Other;
    }

    public async Task<bool> IsVehicleValidAsync(string licensePlate)
    {
        return await _rdwService.VehicleExist(licensePlate);
    }

    public async Task<VehicleTechnicalBriefDtoItem?> GetTechnicalBriefByLicensePlateAsync(string licensePlate)
    {
        var vehicleData = await _rdwService.GetVehicle(licensePlate);
        if (vehicleData?.HasValues != true)
        {
            throw new NotFoundException("Vehicle data not found.");
        }

        var vehicleBrief = MapToVehicleTechnicalBriefDtoItem(licensePlate, vehicleData);
        await PopulateFuelInformation(vehicleBrief);

        return vehicleBrief;
    }

    private VehicleTechnicalBriefDtoItem MapToVehicleTechnicalBriefDtoItem(string licensePlate, JToken vehicleData)
    {
        return new VehicleTechnicalBriefDtoItem
        {
            LicensePlate = licensePlate,
            Brand = vehicleData.GetSafeValue("merk"),
            Model = vehicleData.GetSafeValue("handelsbenaming"),
            YearOfFirstAdmission = vehicleData.GetSafeDateYearValue("datum_eerste_toelating_dt"),
            MOTExpiryDate = vehicleData.GetSafeDateValue("vervaldatum_apk_dt"),
            Mileage = vehicleData.GetSafeValue("tellerstandoordeel")
        };
    }

    private async Task PopulateFuelInformation(VehicleTechnicalBriefDtoItem vehicleBrief)
    {
        var fuelInfo = await _rdwService.GetVehicleFuel(vehicleBrief.LicensePlate);
        if (fuelInfo?.HasValues == true)
        {
            var combinedFuelConsumption = fuelInfo.GetSafeDecimalValue("brandstofverbruik_gecombineerd");
            vehicleBrief.FuelType = fuelInfo.GetSafeValue("brandstof_omschrijving");
            vehicleBrief.CombinedFuelConsumption = combinedFuelConsumption;

            if (combinedFuelConsumption != 0)
            {
                vehicleBrief.FuelEfficiency = 100M / (combinedFuelConsumption * 100M);
            }
        }
    }

    //public async Task<IEnumerable<VehicleServiceLogDtoItem>> GetVehicleServiceLogs(string licensePlate)
    //{
    //    var data = await _rdwService.GetVehicleServiceLogs(licensePlate);
    //    if (data?.HasValues != true)
    //    {
    //        throw new NotFoundException("Vehicle data not found.");
    //    }

    //    var response = new List<VehicleServiceLogDtoItem>();
    //    foreach (var item in data)
    //    {
    //        var date = item.GetSafeDateValue("datum_keuring");
    //        var mileage = item.GetSafeValue("tellerstand");
    //        var result = item.GetSafeValue("resultaat");
    //        var remarks = item.GetSafeValue("opmerkingen");
    //        var serviceLog = new VehicleServiceLogDtoItem
    //        {
    //            Date = date,
    //            Mileage = mileage,
    //            Result = result,
    //            Remarks = remarks
    //        };

    //        response.Add(serviceLog);
    //    }

    //    return response;
    //}

    private VehicleInfoSectionItem GetFiscalData(JToken data)
    {
        return new VehicleInfoSectionItem("Fiscaal")
        {
            Values = new List<List<string>> {
            new() { "Bruto BPM", $"€ {data.GetSafeValue("bruto_bpm")}" },
            new() { "Catalogusprijs", $"€ {data.GetSafeValue("catalogusprijs")}" }
        }
        };
    }

    private VehicleInfoSectionItem GetShaftsData(JArray? shafts)
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

    private VehicleInfoSectionItem GetCharacteristicsData(JToken data)
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

    private VehicleInfoSectionItem GetEmissions(JToken fuelInfo)
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

    private VehicleInfoSectionItem GetEnvironmentalPerformance(JToken fuelInfo)
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

    private VehicleInfoSectionItem GetMotorData(JToken data)
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

    private VehicleInfoSectionItem GetRecallData(JToken data)
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

    private VehicleInfoSectionItem GetVehicleStatus(JToken data)
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

    private VehicleInfoSectionItem GetCounterReadings(JToken data)
    {
        return new VehicleInfoSectionItem("Tellerstanden")
        {
            Values = new List<List<string>>
        {
            new() {"Jaar laatste registratie", $"{data.GetSafeValue("jaar_laatste_registratie_tellerstand")}"},
            new() {"Oordeel", $"{data.GetSafeValue("tellerstandoordeel")}"},
            new() {"Toelichting", _rdwService.GetVehicleCounterReadingsDescription(data.GetSafeValue("code_toelichting_tellerstandoordeel"))}
        }
        };
    }

    private VehicleInfoSectionItem GetWeightsData(JToken data)
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

    private VehicleInfoSectionItem GetExpirationDatesAndHistory(JToken data)
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

    private VehicleInfoSectionItem GetGeneralData(JToken data)
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

    public async Task<RDWVehicleDetectedDefect[]> GetDefectHistoryByLicensePlateAsync(string licensePlate)
    {
        var data = await _rdwService.GetVehicle(licensePlate);
        if (data?.HasValues != true)
        {
            throw new NotFoundException("Vehicle data not found.");
        }

        throw new NotImplementedException();

        return null;
    }

    public async Task<IEnumerable<RDWDetectedDefectDescription>> GetDetectedDefectDescriptionsAsync()
    {
        return await _rdwService.GetDetectedDefectDescriptions();
    }

    public async Task ForEachVehicleInBatches(Func<IEnumerable<RDWVehicle>, Task> onVehicleBatch)
    {
        var limit = 2000;
        var offset = 0;
        var count = 0;

        do
        {
            var items = await _rdwService.GetVehicles(offset, limit);
            await onVehicleBatch(items);

            count += items.Count();
            offset++;
        }
        while (count == (limit * offset));
    }

    public async Task<IEnumerable<RDWVehicleBasics>> GetVehicleBasicsWithMOTRequirement(int offset, int limit)
    {
        return await _rdwService.GetVehicleBasicsWithMOTRequirement(offset, limit);
    }

    public async Task<int> GetVehicleBasicsWithMOTRequirementCount()
    {
        return await _rdwService.GetVehicleBasicsWithMOTRequirementCount();
    }

    //public async Task<List<VehicleTimelineItem>> GetVehicleUpdatedTimeline(
    //    List<VehicleTimelineItem> timeline, 
    //    RDWVehicleBasics vehicle, 
    //    IEnumerable<RDWDetectedDefectDescription> defectDescriptions
    //)
    //{
    //    var items = timeline.DeepClone() ?? new List<VehicleTimelineItem>();
    //    var defects = await _rdwService.GetVehicleDetectedDefects(vehicle.LicensePlate);
    //    if (defects?.Any() == true)
    //    {
    //        var failedMOTs = UndefinedFailedMOTTimelineItems(vehicle.LicensePlate, items, defects, defectDescriptions);
    //        if (failedMOTs?.Any() == true)
    //        {
    //            items.AddRange(failedMOTs);
    //        }
    //    }

    //    var inspections = await _rdwService.GetVehicleInspectionNotifications(vehicle.LicensePlate);
    //    if (inspections?.Any() == true)
    //    {
    //        var succeededMOTs = UndefinedSucceededMOTTimelineItems(vehicle.LicensePlate, items, inspections);
    //        if (succeededMOTs?.Any() == true)
    //        {
    //            items.AddRange(succeededMOTs);
    //        }
    //    }

    //    var ownerChanged = UndefinedOwnerChangedTimelineItem(vehicle.LicensePlate, items, vehicle.RegistrationDateDt);
    //    if (ownerChanged != null)
    //    {
    //        items.Add(ownerChanged);
    //    }

    //    return items;
    //}

    public async Task<(List<VehicleTimelineItem> failedMOTsToInsert, List<VehicleTimelineItem> failedMOTsToUpdate)> FailedMOTTimelineItems(VehicleLookupItem vehicle, IEnumerable<RDWVehicleDetectedDefect> detectedDefects, IEnumerable<RDWDetectedDefectDescription> defectDescriptions)
    {
        var itemsToInsert = new List<VehicleTimelineItem>();
        var itemsToUpdate = new List<VehicleTimelineItem>();

        // No defects found
        if (detectedDefects?.Any() != true)
        {
            return (itemsToInsert, itemsToUpdate);
        }

        var groupedByDate = detectedDefects.GroupBy(x => x.DetectionDate);
        foreach (var group in groupedByDate)
        {
            if (vehicle.Timeline?.Any(x => x.Date == group.Key) == true)
            {
                // Already exists
                continue;
            }

            var item = CreateFailedMOTTimelineItem(vehicle.LicensePlate, group, defectDescriptions);
            itemsToInsert.Add(item);
        }

        return (itemsToInsert, itemsToUpdate);
    }

    public async Task<(List<VehicleTimelineItem> failedMOTsToInsert, List<VehicleTimelineItem> failedMOTsToUpdate)> SucceededMOTTimelineItems(VehicleLookupItem vehicle, IEnumerable<RDWvehicleInspectionNotification> notifications)
    {
        var itemsToInsert = new List<VehicleTimelineItem>();
        var itemsToUpdate = new List<VehicleTimelineItem>();

        // No notifications found
        if (notifications?.Any() != true)
        {
            return (itemsToInsert, itemsToUpdate);
        }

        var items = new List<VehicleTimelineItem>();
        var groupedByDate = notifications.GroupBy(x => x.DateTimeByAuthority);
        foreach (var notification in notifications)
        {
            if (vehicle.Timeline?.Any(x => x.Date == notification!.DateTimeByAuthority) == true)
            {
                // Already exists
                continue;
            }

            var item = CreateSucceededMOTTimelineItem(vehicle.LicensePlate, notification);
            itemsToInsert.Add(item);
        }

        return (itemsToInsert, itemsToUpdate);
    }

    public async Task<VehicleTimelineItem?> OwnerChangedTimelineItem(VehicleLookupItem vehicle)
    {
        var entity = vehicle.Timeline?.FirstOrDefault(x => 
            x.Type == VehicleTimelineType.OwnerChange &&
            x.Date == vehicle.DateOfAscription
        );

        // Already exists or has invalid date
        if (entity != null || vehicle.DateOfAscription == null || vehicle.DateOfAscription == DateTime.MinValue)
        {
            return null;
        }

        var item = CreateOwnerChangeTimelineItem(vehicle.LicensePlate, (DateTime)vehicle.DateOfAscription);
        return item;
    }

    private VehicleTimelineItem CreateFailedMOTTimelineItem(string licensePlate, IGrouping<DateTime, RDWVehicleDetectedDefect> group, IEnumerable<RDWDetectedDefectDescription> defectDescriptions)
    {
        var timelineItem = new VehicleTimelineItem()
        {
            Id = Guid.NewGuid(),
            VehicleLicensePlate = licensePlate,
            Date = group.Key,
            Title = "APK afgekeurd",
            Type = VehicleTimelineType.FailedMOT,
            Priority = VehicleTimelinePriority.Medium,
            ExtraData = new Dictionary<string, string>()
        };

        // avoid repeated deserialization
        var extraData = timelineItem.ExtraData;

        foreach (var defect in group)
        {
            var information = defectDescriptions.First(x => x.Identification == defect.Identifier);
            var description = information.Description;
            if (defect.DetectedAmount > 1)
            {
                description += $" ({defect.DetectedAmount}x)";
            }

            extraData.Add(description, information.DefectArticleNumber);
        }

        // set the property back to serialize and store the updates
        timelineItem.ExtraData = extraData;

        var total = group.Select(x => x.DetectedAmount).Sum();
        timelineItem.Description = $"op {total} plekken";

        return timelineItem;
    }

    private VehicleTimelineItem CreateSucceededMOTTimelineItem(string licensePlate, RDWvehicleInspectionNotification notification)
    {
        var timelineItem = new VehicleTimelineItem()
        {
            Id = Guid.NewGuid(),
            VehicleLicensePlate = licensePlate,
            Date = notification.DateTimeByAuthority,
            Title = "APK goedgekeurd",
            Description = "",
            Type = VehicleTimelineType.SucceededMOT,
            Priority = VehicleTimelinePriority.Medium,
            ExtraData = new Dictionary<string, string>() {
                { "Verval datum", notification.ExpiryDateTime.ToShortDateString() }
            }
        };

        return timelineItem;
    }

    private VehicleTimelineItem CreateOwnerChangeTimelineItem(string licensePlate, DateTime dateOfAscription)
    {
        var timelineItem = new VehicleTimelineItem()
        {
            Id = Guid.NewGuid(),
            VehicleLicensePlate = licensePlate,
            Date = dateOfAscription,
            Title = "Nieuwe eigenaar",
            Description = "",
            Type = VehicleTimelineType.OwnerChange,
            Priority = VehicleTimelinePriority.Low,
            ExtraData = new Dictionary<string, string>()
        };

        return timelineItem;
    }

    public bool MOTIsRequired(string europeanVehicleCategory)
    {
        return _rdwService.MOTIsRequired(europeanVehicleCategory);
    }

    public async Task<IEnumerable<RDWVehicleDetectedDefect>> GetVehicleDetectedDefects(List<string> licensePlates)
    {
        var limit = 2000;
        var offset = 0;

        var defects = new List<RDWVehicleDetectedDefect>();
        do
        {
            var items = await _rdwService.GetVehicleDetectedDefects(licensePlates, offset, limit);
            defects.AddRange(items);
            offset++;
        }
        while (defects.Count() == (limit * offset));
        return defects.ToArray();
    }

    public async Task<IEnumerable<RDWvehicleInspectionNotification>> GetVehicleInspectionNotifications(List<string> licensePlates)
    {
        var limit = 2000;
        var offset = 0;

        var inspections = new List<RDWvehicleInspectionNotification>();
        do
        {
            var items = await _rdwService.GetVehicleInspectionNotifications(licensePlates, offset, limit);
            inspections.AddRange(items);
            offset++;
        }
        while (inspections.Count() == (limit * offset));
        return inspections.ToArray();
    }

}
