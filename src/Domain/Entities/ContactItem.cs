using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AutoHelper.Domain.Entities;

public class ContactItem : BaseAuditableEntity
{
    [Required]
    public string FullName { get; set; }

    public string PhoneNumber { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    public string Responsibility { get; set; }

    //public Guid GarageItemId { get; set; }
    //[ForeignKey("GarageItemId")]
    //public GarageItem Garage { get; set; }
}
