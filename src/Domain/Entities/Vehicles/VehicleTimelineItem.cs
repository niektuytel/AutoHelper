using System.ComponentModel.DataAnnotations;

namespace AutoHelper.Domain.Entities.Vehicles;

public class VehicleTimelineItem: BaseEntity
{
    public VehicleTimelineItem()
    { }

    [Required]
    public string Title { get; set; } = null!;

    [Required]
    public string Description { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public VehicleTimelineType Type { get; set; }

    public string ExtraInformationJson { get; set; } = "";
}