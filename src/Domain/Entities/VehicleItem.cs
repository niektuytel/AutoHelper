using System.ComponentModel.DataAnnotations.Schema;

namespace AutoHelper.Domain.Entities;

public class VehicleItem : BaseAuditableEntity
{
    public string LicensePlate { get; set; }

    public string Model { get; set; }

    public string Brand { get; set; }

    public DateTime RegistrationDate { get; set; }

    //public Guid GarageItemId { get; set; }
    //[ForeignKey("GarageItemId")]
    //public GarageItem Garage { get; set; }

    public VehicleOwnerItem VehicleOwner { get; set; }

    //public ICollection<MaintenanceItem> Maintenances { get; set; }

    //public ICollection<OrderItem> Orders { get; set; }
}
