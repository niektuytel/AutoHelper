using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace AutoHelper.Domain.Entities.Vehicles;

public class VehicleTimelineItem: BaseEntity
{
    public VehicleTimelineItem()
    { }

    [Required]
    public string VehicleLicensePlate { get; set; }

    [ForeignKey(nameof(VehicleLicensePlate))]
    public VehicleLookupItem VehicleLookup { get; set; }

    /// <summary>
    /// Only defined if the timeline item is related to a service log item
    /// </summary>
    [Required]
    public Guid? VehicleServiceLogId { get; set; }

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
    private List<Tuple<string, string>> _extraDataCache;

    public string ExtraDataTableJson { get; private set; } = "{}";

    [NotMapped]
    public List<Tuple<string, string>> ExtraData
    {
        get
        {
            if (_extraDataCache == null)
            {
                _extraDataCache = JsonConvert.DeserializeObject<List<Tuple<string, string>>>(ExtraDataTableJson) ?? new List<Tuple<string, string>>();
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