using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain.Entities;

namespace AutoHelper.Application.Vehicles.Queries.GetVehicleBriefInfo;
public class VehicleBriefDtoItem
{
    public string LicensePlate { get; set; }

    public string Brand { get; set; }

    public string Consumption { get; set; }
    
    public string Mileage { get; set; }

    public DateTime MOTExpiryDate { get; set; }

    public DateTime DateOfAscription { get; set; }

}
