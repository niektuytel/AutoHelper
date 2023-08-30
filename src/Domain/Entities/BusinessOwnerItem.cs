using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AutoHelper.Domain.Entities;

public class BusinessOwnerItem : BaseEntity
{
    [Required]
    public string FullName { get; set; }

    public string PhoneNumber { get; set; }

    public string Email { get; set; }

    //[InverseProperty("BusinessOwner")]
    //public ICollection<GarageItem> Garages { get; set; }
}
