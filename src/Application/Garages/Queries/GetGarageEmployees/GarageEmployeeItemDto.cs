using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain.Entities;

namespace AutoHelper.Application.Garages.Queries.GetGarageEmployees;

public class GarageEmployeeItemDto : IMapFrom<GarageEmployeeItem>
{
    public Guid Id { get; set; }


}
