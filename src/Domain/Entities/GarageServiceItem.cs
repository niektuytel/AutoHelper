using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AutoHelper.Domain.Entities;

public class GarageServiceItem : BaseAuditableEntity
{
    [Required]
    public string ServiceName { get; set; } // Like "Oil Change", "Wheel Alignment", etc.

    public string Description { get; set; }

    //public decimal Price { get; set; }

    //public Guid GarageItemId { get; set; }
    //[ForeignKey("GarageItemId")]
    //public GarageItem Garage { get; set; }
}
