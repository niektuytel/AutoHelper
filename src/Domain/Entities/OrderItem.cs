using System.ComponentModel.DataAnnotations.Schema;

namespace AutoHelper.Domain.Entities;

public class OrderItem : BaseAuditableEntity
{
    public string Description { get; set; } // What needs to be fixed or serviced.

    public DateTime OrderDate { get; set; }

    //public Guid VehicleId { get; set; }
    //[ForeignKey("VehicleId")]
    //public VehicleItem Vehicle { get; set; }

    //public Guid VehicleOwnerId { get; set; }
    //[ForeignKey("VehicleOwnerId")]
    //public VehicleOwnerItem VehicleOwner { get; set; }
}
