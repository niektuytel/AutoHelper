using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AutoHelper.Domain.Entities.Deprecated;

public class ContactItem : BaseEntity
{
    [Required]
    public string FullName { get; set; }

    public string PhoneNumber { get; set; }

    [EmailAddress]
    public string Email { get; set; }

}
