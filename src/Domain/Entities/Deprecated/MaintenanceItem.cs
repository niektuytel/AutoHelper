using System.ComponentModel.DataAnnotations.Schema;

namespace AutoHelper.Domain.Entities.Deprecated;

public class MaintenanceItem : BaseAuditableEntity
{
    public string Description { get; set; }

    public DateTime MaintenanceDate { get; set; }

    //public Guid VehicleId { get; set; }
    //[ForeignKey("VehicleId")]
    //public VehicleItem Vehicle { get; set; }
}
