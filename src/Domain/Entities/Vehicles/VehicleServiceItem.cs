using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AutoHelper.Domain.Entities.Vehicles;

public class VehicleServiceItem : BaseAuditableEntity
{
    /// <summary>
    /// Like: Inspection, Repair, Maintenance, etc.
    /// </summary>
    public GarageServiceType Type { get; set; } = GarageServiceType.Other;

    /// <summary>
    /// Like: "Change the oil in the engine", "Align the wheels", etc.
    /// </summary>
    public string Description { get; set; }

}
