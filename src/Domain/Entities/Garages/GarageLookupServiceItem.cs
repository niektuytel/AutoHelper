using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AutoHelper.Domain.Entities.Vehicles;

namespace AutoHelper.Domain.Entities.Garages;

public class GarageLookupServiceItem : BaseEntity
{
    [Required]
    public string GarageLookupIdentifier { get; set; }

    [ForeignKey(nameof(GarageLookupIdentifier))]
    public GarageLookupItem? GarageLookup { get; set; } = null!;

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
    public VehicleType VehicleType { get; set; } = VehicleType.Any;

    /// <summary>
    /// Like: "MOT Service", "Oil Change", "Wheel Alignment", etc.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Like: "Change the oil in the engine", "Align the wheels", etc.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Expected next date
    /// </summary>
    public bool ExpectedNextDateIsRequired { get; set; } = false;

    /// <summary>
    /// Expected next odometer readings
    /// </summary>
    public bool ExpectedNextOdometerReadingIsRequired { get; set; } = false;
}
