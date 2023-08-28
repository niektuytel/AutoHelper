using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AutoHelper.Domain.Entities;

public class GarageEmployeeItem : BaseAuditableEntity
{
    [Required]
    public string FullName { get; set; }

    public string Position { get; set; } // Like "Mechanic", "Manager", etc.

    public DateTime DateOfHire { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    //public Guid GarageItemId { get; set; }
    //[ForeignKey("GarageItemId")]
    //public GarageItem Garage { get; set; }
}
