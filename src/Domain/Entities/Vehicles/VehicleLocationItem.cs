using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AutoHelper.Domain.Entities.Vehicles;

public class VehicleLocationItem : BaseEntity
{
    public string? Address { get; set; }

    public string? City { get; set; }

    public string? PostalCode { get; set; }

    public string? Country { get; set; }

    [Required]
    public float Longitude { get; set; }

    [Required]
    public float Latitude { get; set; }

}
