using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AutoHelper.Domain.Entities.Conversations;
using NetTopologySuite.Geometries;

namespace AutoHelper.Domain.Entities.Vehicles;

public class VehicleLookupItem
{
    public VehicleLookupItem()
    { }

    [Key]
    [Required]
    public string LicensePlate { get; set; }

    [Required]
    public DateTime? DateOfMOTExpiry { get; set; }

    [Required]
    public DateTime? DateOfAscription { get; set; }

    public Geometry? Location { get; set; }

    public string? ReporterPhoneNumber { get; set; }

    public string? ReporterWhatsappNumber { get; set; }

    public string? ReporterEmailAddress { get; set; }

    [Required]
    public DateTime Created { get; set; }

    public string? CreatedBy { get; set; }

    [Required]
    public DateTime? LastModified { get; set; }

    public string? LastModifiedBy { get; set; }

    [Required]
    public List<VehicleTimelineItem> Timeline { get; set; } = new List<VehicleTimelineItem>();

    [Required]
    public ICollection<ConversationItem> Conversations { get; set; } = new List<ConversationItem>();

    [Required]
    public ICollection<VehicleServiceLogItem> ServiceLogs { get; set; } = new List<VehicleServiceLogItem>();

}
