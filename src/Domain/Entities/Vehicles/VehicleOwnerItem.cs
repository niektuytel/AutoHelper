using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AutoHelper.Domain.Entities.Vehicles;

public class VehicleOwnerItem : BaseAuditableEntity
{
    [Required]
    public string FullName { get; set; }

    [Required]
    public string Email { get; set; }

    public string? PhoneNumber { get; set; }

    public VehicleOwnerLocationItem? VehicleOwnerLocation { get; set; }
}
