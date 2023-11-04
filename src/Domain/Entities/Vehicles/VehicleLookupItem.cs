using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AutoHelper.Domain.Entities.Conversations;
using NetTopologySuite.Geometries;

namespace AutoHelper.Domain.Entities.Vehicles;

public class VehicleLookupItem : BaseAuditableEntity
{
    public VehicleLookupItem()
    { }


    [Required]
    public string LicensePlate { get; set; }

    public DateTime? MOTExpiryDate { get; set; }

    public DateTime DateOfAscription { get; set; }

    public Geometry? Location { get; set; }

    public string? PhoneNumber { get; set; }

    public string? WhatsappNumber { get; set; }

    public string? EmailAddress { get; set; }


    [Required]
    public ICollection<VehicleTimelineItem> Timeline { get; set; } = new List<VehicleTimelineItem>();

    [Required]
    public ICollection<ConversationItem> Conversations { get; set; } = new List<ConversationItem>();

    [Required]
    public ICollection<VehicleServiceLogItem> ServiceLogs { get; set; } = new List<VehicleServiceLogItem>();

}
