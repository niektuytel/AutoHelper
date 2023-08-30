using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AutoHelper.Domain.Entities;

public class LocationItem : BaseEntity
{
    [Required]
    public double Longitude { get; set; }

    [Required]
    public double Latitude { get; set; }

    public string Address { get; set; }

    public string City { get; set; }

    public string PostalCode { get; set; }

    public string Country { get; set; }

    //[InverseProperty("Location")]
    //public GarageItem Garage { get; set; }
}
