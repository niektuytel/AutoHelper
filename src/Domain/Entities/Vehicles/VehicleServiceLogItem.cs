using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;
using AutoHelper.Domain.Entities.Garages;

namespace AutoHelper.Domain.Entities.Vehicles;

public class VehicleServiceLogItem: BaseAuditableEntity
{
    public VehicleServiceLogItem()
    {
    }

    [Required]
    public string VehicleLicensePlate { get; set; }
    
    [ForeignKey(nameof(VehicleLicensePlate))]
    public VehicleLookupItem VehicleLookup { get; set; }

    [Required]
    public string GarageLookupIdentifier { get; set; }

    [ForeignKey(nameof(GarageLookupIdentifier))]
    public GarageLookupItem? GarageLookup { get; set; } = null!;

    public Guid? GarageServiceId { get; set; }

    [Required]
    public GarageServiceType Type { get; set; } = GarageServiceType.Other;

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? AttachedFile { get; set; }

    public string Notes { get; set; } = "";

    [Required]
    public DateTime Date { get; set; }

    public DateTime? ExpectedNextDate { get; set; } = null!;

    [Required]
    public int OdometerReading { get; set; }

    public int? ExpectedNextOdometerReading { get; set; } = null!;

    [Required]
    public VehicleServiceLogStatus Status { get; set; } = VehicleServiceLogStatus.NotVerified;

    [Required]
    public string ReporterName { get; set; } = null!;

    public string? ReporterPhoneNumber { get; set; } = null!;

    public string? ReporterEmailAddress { get; set; } = null!;

    public string MetaData { get; set; } = "";

}

