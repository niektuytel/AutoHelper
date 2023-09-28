using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoHelper.Domain.Entities.Vehicles;

public class VehicleItem : BaseAuditableEntity
{
    public VehicleItem()
    {
        ServiceLogs = new List<VehicleServiceLogItem>();
    }

    [Required]
    public string LicensePlate { get; set; }

    [Required]
    public DateTime MOTExpiryDate { get; set; }

    [Required]
    public VehicleLocationItem LastLocation { get; set; }

    public VehicleOwnerItem? LastVehicleOwner { get; set; }

    [Required]
    public ICollection<VehicleServiceLogItem> ServiceLogs { get; set; }

}
