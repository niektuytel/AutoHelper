using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace AutoHelper.Domain.Entities.Vehicles;

public class VehicleTimelineItem: BaseEntity
{
    public VehicleTimelineItem()
    { }

    [Required]
    public DateTime LatestChange { get; set; }

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

    [NotMapped]
    private Dictionary<string, string> _extraDataCache;

    public string ExtraDataTableJson { get; private set; } = "{}";

    [NotMapped]
    public Dictionary<string, string> ExtraData
    {
        get
        {
            if (_extraDataCache == null)
            {
                _extraDataCache = JsonConvert.DeserializeObject<Dictionary<string, string>>(ExtraDataTableJson) ?? new Dictionary<string, string>();
            }
            return _extraDataCache;
        }
        set
        {
            _extraDataCache = value;
            ExtraDataTableJson = JsonConvert.SerializeObject(value);
        }
    }

}