using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

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

    [Required]
    public VehicleTimelinePriority Priority { get; set; } = VehicleTimelinePriority.Low;

    public string ExtraDataTableJson { get; private set; } = "";

    [NotMapped]
    public Dictionary<string, string> ExtraData
    {
        get => JsonConvert.DeserializeObject<Dictionary<string, string>>(ExtraDataTableJson) ?? new();
        set => ExtraDataTableJson = JsonConvert.SerializeObject(value);
    }


}