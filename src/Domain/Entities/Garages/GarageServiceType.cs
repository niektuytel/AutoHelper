namespace AutoHelper.Domain.Entities.Garages;

public enum GarageServiceType
{
    /// <summary>
    /// Some other service type
    /// </summary>
    Other = 0,
              
    /// <summary>
    /// Maintenance, onderhoud used for example for oil change
    /// </summary>
    Service = 10,

    /// <summary>
    /// Repair, reparatie used for example for replacing a broken part
    /// </summary>
    Repair = 20,

    /// <summary>
    /// Inspection, inspectie used for example for a problem inspection or a APK inspection
    /// </summary>
    Inspection = 30,
}