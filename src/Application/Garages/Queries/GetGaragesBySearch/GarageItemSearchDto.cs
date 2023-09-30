﻿using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain.Entities.Garages;

namespace AutoHelper.Application.Garages.Queries.GetGaragesBySearch;

public class GarageItemSearchDto: IMapFrom<GarageItem>
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    //public GarageLocationItem Location { get; set; }

    //public ICollection<GarageEmployeeItemSearchDto> Employees { get; set; }
}