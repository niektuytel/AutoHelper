using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;
using AutoHelper.Domain.Entities.Garages;

namespace AutoHelper.Domain.Entities.Vehicles;

public class VehicleServiceLogItem: BaseAuditableEntity
{
    public VehicleServiceLogItem()
    {
        ServiceItems = new List<VehicleServiceItem>();
    }

    [Required]
    public string VehicleLicensePlate { get; set; }
    
    [ForeignKey(nameof(VehicleLicensePlate))]
    public VehicleLookupItem VehicleLookup { get; set; }

    [Required]
    public Guid PerformedByGarageId { get; set; }

    [ForeignKey(nameof(PerformedByGarageId))]
    public GarageItem? PerformedByGarage { get; set; }

    [Required]
    public DateTime ServiceDate { get; set; }

    [Required]
    public int OdometerReading { get; set; }

    public string? WorkDescription { get; set; }

    public string? Notes { get; set; }

    public VehicleServiceLogVerificationItem? Verification { get; set; }

    public ICollection<Attachment> Attachments { get; set; }

    [Required]
    public ICollection<VehicleServiceItem>? ServiceItems { get; set; }

    public string MetaData { get; set; } = "";

    // TODO: privacy reasons, into meta data? (still need it as garage can still give an special price)
    public decimal? TotalCost { get; set; }

    // TODO: only used for mot or regular service, into meta data?
    public int? ExpectedNextServiceOdometerReading { get; set; }
    public DateTime? ExpectedNextServiceDate { get; set; }

}

