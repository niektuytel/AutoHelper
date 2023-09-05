using System.ComponentModel.DataAnnotations;

namespace AutoHelper.Application.Garages.Commands.CreateGarageItem;

public class BriefLocationDto
{
    public string Address { get; set; }

    public string PostalCode { get; set; }

    public string City { get; set; }

    public string Country { get; set; }

    public double Longitude { get; set; }

    public double Latitude { get; set; }
}