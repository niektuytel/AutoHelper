namespace AutoHelper.Domain.Entities.Garages;

public enum GarageServiceType
{
    /// <summary>
    /// Some other service type
    /// </summary>
    Other = 0,
                                       
    /// <summary>
    /// RDW RELATED, Bedrijfsvoorraad: De voorraad van voertuigen die een dealer of garage op voorraad heeft, klaar voor verkoop of lease.
    /// </summary>
    CompanyStockService = 30,

    /// <summary>
    /// RDW RELATED, Tenaamstellen: Het officieel registreren van een voertuig op naam van een persoon of bedrijf.
    /// </summary>
    RegistrationService = 40,

    /// <summary>
    /// RDW RELATED, Versnelde inschrijving: Een snellere procedure voor het registreren of inschrijven van een voertuig in het kentekenregister.
    /// </summary>
    AcceleratedRegistrationService = 50,

    /// <summary>
    /// RDW RELATED, APK Licht voertuig: Periodieke keuring voor lichte voertuigen om de veiligheid en emissies te controleren.
    /// </summary>
    MOTServiceLightVehicle = 60,

    /// <summary>
    /// RDW RELATED, APK Zwaar voertuig: Periodieke keuring voor zware voertuigen, vergelijkbaar met de APK voor lichte voertuigen, maar met specifieke criteria voor zware voertuigen.
    /// </summary>
    MOTServiceHeavyVehicle = 61,

    /// <summary>
    /// RDW RELATED, APK-Landbouw: Periodieke keuring voor landbouwvoertuigen, als deze in de toekomst onder de APK-plicht vallen.
    /// </summary>
    MOTServiceAgriculture = 62,

    /// <summary>
    /// RDW RELATED, Controleapparaten: Dit kan verwijzen naar het controleren of installeren van tachografen of andere apparatuur die de activiteit van het voertuig registreert.
    /// </summary>
    ControlDeviceService = 70,

    /// <summary>
    /// RDW RELATED, Gasinstallaties: Installatie, onderhoud of inspectie van LPG- of CNG-installaties in voertuigen.
    /// </summary>
    GasInstallationService = 80,

    /// <summary>
    /// RDW RELATED, Ombouwmelding Snorfiets: Wijzigingen aan een snorfiets melden, bijvoorbeeld als deze is omgebouwd tot een bromfiets of andersom.
    /// </summary>
    MopedConversionService = 90,

    /// <summary>
    /// RDW RELATED, Demontage: Dit kan het demonteren van voertuigen zijn voor onderdelen of reparatie.
    /// </summary>
    DismantlingService = 100,

    /// <summary>
    /// RDW RELATED, Boordcomputertaxi: Installatie, onderhoud of inspectie van boordcomputers voor taxi's.
    /// </summary>
    TaxiComputerService = 110,

    /// <summary>
    /// RDW RELATED, Kentekenplaatfabrikant: Terwijl sommige garages kentekenplaten kunnen maken, is dit voornamelijk een taak voor gespecialiseerde bedrijven.
    /// </summary>
    LicensePlateManufactureService = 120,


    Inspection = 131,                     // Probleem inspectie
    SmallMaintenance = 132,               // Kleine beurt
    GreatMaintenance = 133,               // Grote beurt
    AirConditioningMaintenance = 134,     // Airco onderhoud
    SeasonalTireChange = 135,             // Seizoens bandenwissel

}