using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain.Entities;

namespace AutoHelper.Application.Vehicles._DTOs;


public class VehicleSpecificationsDtoItem
{
    public List<VehicleInfoSectionItem> Data { get; set; } = new List<VehicleInfoSectionItem>();
}

public class VehicleInfoSectionItem
{
    public VehicleInfoSectionItem(string title)
    {
        Title = title;
    }

    public string Title { get; private set; }

    public List<List<string>> Values { get; set; }

}