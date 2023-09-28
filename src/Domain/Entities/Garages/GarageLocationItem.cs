using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AutoHelper.Domain.Entities.Garages;

public class GarageLocationItem : BaseEntity
{
    [Required]
    public string Address { get; set; }

    [Required]
    public string City { get; set; }

    [Required]
    public string PostalCode { get; set; } = "";

    [Required]
    public string Country { get; set; }

    [Required]
    public float Longitude { get; set; }

    [Required]
    public float Latitude { get; set; }

}
