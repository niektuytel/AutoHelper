using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AutoHelper.Domain.Entities;

public class VehicleOwnerItem : BaseAuditableEntity
{
    [Required]
    public string FullName { get; set; }

    public string PhoneNumber { get; set; }

    public string Email { get; set; }

    //[InverseProperty("VehicleOwner")]
    //public ICollection<VehicleItem> Vehicles { get; set; }
}
