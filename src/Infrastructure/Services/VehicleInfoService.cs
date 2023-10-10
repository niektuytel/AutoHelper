using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles.Queries.GetVehicleBriefInfo;
using AutoHelper.Application.Vehicles.Queries.GetVehicleInfo;
using AutoHelper.Application.Vehicles.Queries.GetVehicleRelatedServices;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Infrastructure.Common.Extentions;
using Azure;
using MediatR;
using Newtonsoft.Json.Linq;

namespace AutoHelper.Infrastructure.Services;

internal class VehicleInfoService : IVehicleInfoService
{
    private readonly RDWApiClient _rdwService;

    public VehicleInfoService(RDWApiClient rdwService)
    {
        _rdwService = rdwService;
    }

    public async Task<bool> ValidVehicle(string licensePlate)
    {
        return await _rdwService.VehicleExist(licensePlate);
    }

    public async Task<VehicleBriefInfoItemDto?> GetVehicleBriefInfo(string licensePlate)
    {
        var data = await _rdwService.GetVehicle(licensePlate);
        if (data?.HasValues != true)
        {
            throw new NotFoundException("Vehicle data not found.");
        }

        var from = data.GetSafeDateYearValue("datum_eerste_toelating_dt");
        var fromText = from != 0 ? $" uit {from}" : string.Empty;
        var brandText = $"{data.GetSafeValue("merk")} ({data.GetSafeValue("handelsbenaming")}){fromText}";
        var motExpirydate = data.GetSafeDateValue("vervaldatum_apk_dt");
        var mileage = data.GetSafeValue("tellerstandoordeel");
        var response = new VehicleBriefInfoItemDto
        {
            LicensePlate = licensePlate,
            Brand = brandText,
            MOTExpiryDate = motExpirydate,
            Mileage = mileage
        };

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

    public async Task<VehicleInfoItemDto> GetVehicleInfoQuery(string licensePlate)
    {
        var response = new VehicleInfoItemDto();
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

    /// <summary>
    /// Decide which services are available for a vehicle based on the data from the RDW
    /// </summary>
    public async Task<GarageServiceType[]> GetRelatedServiceTypesByLicencePlate(string licensePlate)
    {
        var data = await _rdwService.GetVehicle(licensePlate);
        var serviceTypes = new List<GarageServiceType>();

        if (int.TryParse(data?["technische_max_massa_voertuig"]?.ToString(), out int weight) && weight > 3500)
        {
            serviceTypes.Add(GarageServiceType.MOTServiceHeavyVehicle);
        }
        else
        {
            serviceTypes.Add(GarageServiceType.MOTServiceLightVehicle);
        }

        return serviceTypes.ToArray();
    }

    // TODO
    //Lichte Voertuigen Relevant:

    //Fotograaf bemand: Voor identificatiedoeleinden of documentatie van lichte voertuigen.
    //APK Lichte voertuigen: Verplichte keuring voor lichte voertuigen.
    //Bedrijfsvoorraad: Kan lichte voertuigen bevatten die klaar zijn voor verkoop of lease.
    //Handelaarskenteken: Voor voertuigbedrijven om met lichte voertuigen te rijden zonder ze op naam te zetten.
    //Tenaamstellen: Registratie van lichte voertuigen op naam van een persoon of bedrijf.
    //Controleapparaten: Apparaten zoals tachografen die in lichte voertuigen kunnen worden gebruikt.
    //Uitvoer: Voor het exporteren van lichte voertuigen uit Nederland.
    //Versnelde inschrijving: Snellere procedure voor lichte voertuigen.
    //Ombouwmelding Snorfiets: Wijzigingen aan een snorfiets melden.
    //Gasinstallaties: Diensten voor het installeren of repareren van gasinstallaties in lichte voertuigen.
    //Kentekenplaatfabrikant: Produceert kentekenplaten voor lichte voertuigen.
    //Fotograaf onbemand: Voor het vastleggen van lichte voertuigen.
    //Kentekenloket: Voor zaken gerelateerd aan registratie van lichte voertuigen en kentekens.

    //Zware Voertuigen Relevant:

    //APK Zware voertuigen: Verplichte keuring voor zware voertuigen.
    //Bedrijfsvoorraad: Kan zware voertuigen bevatten die klaar zijn voor verkoop of lease.
    //Handelaarskenteken: Voor voertuigbedrijven om met zware voertuigen te rijden zonder ze op naam te zetten.
    //Tenaamstellen: Registratie van zware voertuigen op naam van een persoon of bedrijf.
    //Controleapparaten: Apparaten zoals tachografen die in zware voertuigen worden gebruikt.
    //Uitvoer: Voor het exporteren van zware voertuigen uit Nederland.
    //Demontage: Het proces van het uit elkaar halen van zware voertuigen.
    //Gasinstallaties: Diensten voor het installeren of repareren van gasinstallaties in zware voertuigen.
    //Boordcomputertaxi: Mogelijk gebruikt in zware voertuigen zoals bussen.
    //Kentekenplaatfabrikant: Produceert kentekenplaten voor zware voertuigen.
    //Kentekenloket: Voor zaken gerelateerd aan registratie van zware voertuigen en kentekens.

    //Landbouw Relevant:

    //APK-Landbouw: Verplichte keuring voor landbouwvoertuigen.
    //Bedrijfsvoorraad: Kan landbouwvoertuigen bevatten die klaar zijn voor gebruik.
    //Handelaarskenteken: Voor bedrijven om met landbouwvoertuigen te rijden zonder ze op naam te zetten.
    //Tenaamstellen: Registratie van landbouwvoertuigen op naam van een persoon of bedrijf.
    //Kentekenplaatfabrikant: Produceert kentekenplaten voor landbouwvoertuigen.
    //Kentekenloket: Voor zaken gerelateerd aan registratie van landbouwvoertuigen en kentekens.
    //Let op: Sommige items kunnen relevant zijn voor meer dan één categorie, en sommige items zijn mogelijk niet strikt relevant voor een specifieke categorie maar zijn toch opgenomen voor volledigheid.

}
