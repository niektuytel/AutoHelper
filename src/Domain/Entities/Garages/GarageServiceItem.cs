using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AutoHelper.Domain.Entities.Vehicles;

namespace AutoHelper.Domain.Entities.Garages;

public class GarageServiceItem : BaseEntity
{
    /// <summary>
    /// UserId of the garage owner
    /// </summary>
    [Required]
    public string UserId { get; set; }

    /// <summary>
    /// GarageId for the full garage foreign data
    /// </summary>
    [Required]
    public Guid GarageId { get; set; }

    [ForeignKey(nameof(GarageId))]
    public GarageItem Garage { get; set; }

    /// <summary>
    /// Like: Inspection, Repair, Maintenance, etc.
    /// </summary>
    [Required]
    public GarageServiceType Type { get; set; } = GarageServiceType.Other;
    
    /// <summary>
    /// Like: Light Vehicle, Heavy vehicle, etc.
    /// Needed for filtering and made able to make more specific services for each vehicle type
    /// </summary>
    [Required]
    public VehicleType VehicleType { get; set; } = VehicleType.Other;

    /// <summary>
    /// Like: "MOT Service", "Oil Change", "Wheel Alignment", etc.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Like: "Change the oil in the engine", "Align the wheels", etc.
    /// </summary>
    public string? Description { get; set; }

}
