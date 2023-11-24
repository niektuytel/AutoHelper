using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Vehicles;

namespace AutoHelper.Application.Vehicles._DTOs;
public class VehicleSpecificationsCardItem
{
    public string LicensePlate { get; set; }

    public VehicleLookupType Type { get; set; }

    public string Brand { get; set; }

    public string Consumption { get; set; }

    public string Mileage { get; set; }

    public DateTime? DateOfMOTExpiry { get; set; } = null;

    public DateTime? DateOfAscription { get; set; } = null;


}
