using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;

namespace AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;


public class VehicleServiceLogItemDto: IMapFrom<VehicleServiceLogItem>
{
    public DateTime Date { get; set; }

    public int Mileage { get; set; }

    public string? Description { get; set; }

    public ICollection<VehicleGarageServiceItemDto> ServiceItems { get; set; }
}
