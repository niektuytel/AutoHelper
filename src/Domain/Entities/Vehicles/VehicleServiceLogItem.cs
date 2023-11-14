using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;
using AutoHelper.Domain.Entities.Garages;

namespace AutoHelper.Domain.Entities.Vehicles;

public class VehicleServiceLogItem: BaseAuditableEntity
{
    public VehicleServiceLogItem()
    {
        Verification = new VehicleServiceLogVerificationItem();
    }

    [Required]
    public string VehicleLicensePlate { get; set; }
    
    [ForeignKey(nameof(VehicleLicensePlate))]
    public VehicleLookupItem VehicleLookup { get; set; }

    [Required]
    public string PerformedByGarageName { get; set; }

    public Guid? PerformedByGarageId { get; set; } = null!;

    [ForeignKey(nameof(PerformedByGarageId))]
    public GarageItem? PerformedByGarage { get; set; } = null!;

    [Required]
    public VehicleServiceLogVerificationItem Verification { get; set; }

    [Required]
    public GarageServiceType Type { get; set; } = GarageServiceType.Other;

    [Required]
    public DateTime Date { get; set; }

    public DateTime? ExpectedNextDate { get; set; } = null!;

    [Required]
    public int OdometerReading { get; set; }

    public int? ExpectedNextOdometerReading { get; set; } = null!;

    public string? Description { get; set; }

    public string? Notes { get; set; }

    public ICollection<VehicleServiceAttachmentItem> Attachments { get; set; }

    public string MetaData { get; set; } = "";

}

