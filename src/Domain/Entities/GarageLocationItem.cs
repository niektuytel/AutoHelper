using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AutoHelper.Domain.Entities;

public class GarageLocationItem : BaseEntity
{
    public string Address { get; set; }

    public string City { get; set; }

    public string PostalCode { get; set; } = "";

    public string Country { get; set; }

    public double Longitude { get; set; }

    public double Latitude { get; set; }

}
