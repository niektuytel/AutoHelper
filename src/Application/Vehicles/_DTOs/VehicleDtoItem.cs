using Newtonsoft.Json;

namespace AutoHelper.Infrastructure.Services;

public class VehicleDtoItem
{
    [JsonProperty("kenteken")]
    public string LicensePlate { get; set; }

    [JsonProperty("voertuigsoort")]
    public string VehicleType { get; set; }

    [JsonProperty("merk")]
    public string Brand { get; set; }

    [JsonProperty("handelsbenaming")]
    public string TradeName { get; set; }

    [JsonProperty("vervaldatum_apk")]
    public string MOTExpiryDate { get; set; }

    [JsonProperty("datum_tenaamstelling")]
    public string RegistrationDate { get; set; }

    [JsonProperty("bruto_bpm")]
    public string GrossVehicleTax { get; set; }

    [JsonProperty("inrichting")]
    public string Configuration { get; set; }

    [JsonProperty("aantal_zitplaatsen")]
    public string NumberOfSeats { get; set; }

    [JsonProperty("eerste_kleur")]
    public string PrimaryColor { get; set; }

    [JsonProperty("tweede_kleur")]
    public string SecondaryColor { get; set; }

    [JsonProperty("aantal_cilinders")]
    public string NumberOfCylinders { get; set; }

    [JsonProperty("cilinderinhoud")]
    public string CylinderCapacity { get; set; }

    [JsonProperty("massa_ledig_voertuig")]
    public string CurbWeight { get; set; }

    [JsonProperty("toegestane_maximum_massa_voertuig")]
    public string AllowedMaximumMass { get; set; }

    [JsonProperty("massa_rijklaar")]
    public string MassReadyToDrive { get; set; }

    [JsonProperty("maximum_massa_trekken_ongeremd")]
    public string MaxTowingCapacityUnbraked { get; set; }

    [JsonProperty("maximum_trekken_massa_geremd")]
    public string MaxTowingCapacityBraked { get; set; }

    [JsonProperty("datum_eerste_toelating")]
    public string DateOfFirstAdmission { get; set; }

    [JsonProperty("datum_eerste_tenaamstelling_in_nederland")]
    public string DateOfFirstRegistrationInTheNetherlands { get; set; }

    [JsonProperty("wacht_op_keuren")]
    public string AwaitingInspection { get; set; }

    [JsonProperty("catalogusprijs")]
    public string CatalogPrice { get; set; }

    [JsonProperty("wam_verzekerd")]
    public string WAMInsured { get; set; }

    [JsonProperty("aantal_deuren")]
    public string NumberOfDoors { get; set; }

    [JsonProperty("aantal_wielen")]
    public string NumberOfWheels { get; set; }

    [JsonProperty("afstand_hart_koppeling_tot_achterzijde_voertuig")]
    public string DistanceCouplingToRearOfVehicle { get; set; }

    [JsonProperty("afstand_voorzijde_voertuig_tot_hart_koppeling")]
    public string DistanceFrontOfVehicleToCoupling { get; set; }

    [JsonProperty("lengte")]
    public string Length { get; set; }

    [JsonProperty("breedte")]
    public string Width { get; set; }

    [JsonProperty("europese_voertuigcategorie")]
    public string EuropeanVehicleCategory { get; set; }

    [JsonProperty("plaats_chassisnummer")]
    public string ChassisNumberLocation { get; set; }

    [JsonProperty("technische_max_massa_voertuig")]
    public string TechnicalMaxVehicleMass { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("typegoedkeuringsnummer")]
    public string TypeApprovalNumber { get; set; }

    [JsonProperty("variant")]
    public string Variant { get; set; }

    [JsonProperty("uitvoering")]
    public string Execution { get; set; }

    [JsonProperty("volgnummer_wijziging_eu_typegoedkeuring")]
    public string SequenceNumberChangeEUTypeApproval { get; set; }

    [JsonProperty("vermogen_massarijklaar")]
    public string PowerToWeightRatio { get; set; }

    [JsonProperty("wielbasis")]
    public string Wheelbase { get; set; }

    [JsonProperty("export_indicator")]
    public string ExportIndicator { get; set; }

    [JsonProperty("openstaande_terugroepactie_indicator")]
    public string OpenRecallIndicator { get; set; }

    [JsonProperty("taxi_indicator")]
    public string TaxiIndicator { get; set; }

    [JsonProperty("maximum_massa_samenstelling")]
    public string MaximumCompositeMass { get; set; }

    [JsonProperty("aantal_rolstoelplaatsen")]
    public string NumberOfWheelchairPlaces { get; set; }

    [JsonProperty("maximum_ondersteunende_snelheid")]
    public string MaximumAssistingSpeed { get; set; }

    [JsonProperty("jaar_laatste_registratie_tellerstand")]
    public string LastYearOfOdometerRegistration { get; set; }

    [JsonProperty("tellerstandoordeel")]
    public string OdometerReadingAssessment { get; set; }

    [JsonProperty("code_toelichting_tellerstandoordeel")]
    public string ExplanationCodeOdometerAssessment { get; set; }

    [JsonProperty("tenaamstellen_mogelijk")]
    public string PossibilityOfRegistration { get; set; }

    [JsonProperty("vervaldatum_apk_dt")]
    public DateTime MOTExpiryDateDt { get; set; }

    [JsonProperty("datum_tenaamstelling_dt")]
    public DateTime RegistrationDateDt { get; set; }

    [JsonProperty("datum_eerste_toelating_dt")]
    public DateTime DateOfFirstAdmissionDt { get; set; }

    [JsonProperty("datum_eerste_tenaamstelling_in_nederland_dt")]
    public DateTime DateOfFirstRegistrationInTheNetherlandsDt { get; set; }

    [JsonProperty("zuinigheidsclassificatie")]
    public string EfficiencyClassification { get; set; }

    [JsonProperty("api_gekentekende_voertuigen_assen")]
    public string ApiRegisteredVehiclesAxes { get; set; }

    [JsonProperty("api_gekentekende_voertuigen_brandstof")]
    public string ApiRegisteredVehiclesFuel { get; set; }

    [JsonProperty("api_gekentekende_voertuigen_carrosserie")]
    public string ApiRegisteredVehiclesBody { get; set; }

    [JsonProperty("api_gekentekende_voertuigen_carrosserie_specifiek")]
    public string ApiRegisteredVehiclesBodySpecific { get; set; }

    [JsonProperty("api_gekentekende_voertuigen_voertuigklasse")]
    public string ApiRegisteredVehiclesVehicleClass { get; set; }
}