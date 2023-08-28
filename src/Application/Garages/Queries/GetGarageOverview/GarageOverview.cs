using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Deprecated;

namespace AutoHelper.Application.Garages.Queries.GetGarageOverview;

public class GarageOverview : IMapFrom<GarageItem>
{
    public string Name { get; set; }

    public IEnumerable<VehicleItem> Vehicles { get; set; }

    public IEnumerable<GarageEmployeeItem> Employees { get; set; }

}
