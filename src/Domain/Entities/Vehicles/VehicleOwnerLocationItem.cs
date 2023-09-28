using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AutoHelper.Domain.Entities.Vehicles;

public class VehicleOwnerLocationItem : BaseEntity
{
    [Required]
    public string Address { get; set; }
    
    [Required]
    public string City { get; set; }

    public string? PostalCode { get; set; }

    public string? Country { get; set; }

    public float? Longitude { get; set; }

    public float? Latitude { get; set; }
}
