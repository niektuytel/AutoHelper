using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;

namespace AutoHelper.Application.Garages.Queries.GetGarageOverview;

public class GarageOverview : IMapFrom<GarageItem>
{
    public string Name { get; set; }

    public IEnumerable<VehicleLookupItem> Vehicles { get; set; }

    //public IEnumerable<CreateGarageEmployeeDto> Employees { get; set; }

}
