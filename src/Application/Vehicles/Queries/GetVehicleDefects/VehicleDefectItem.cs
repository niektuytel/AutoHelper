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
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;

namespace AutoHelper.Application.Vehicles.Queries.GetVehicleDefects;


public class VehicleDefectItem
{
    public string Id { get; set; }

    public int StartDate { get; set; }

    public int EndDate { get; set; }
    
    public int ParagraphNumber { get; set; }
    
    public string ArticleNumber { get; set; }
    
    public string? Description { get; set; }
}
