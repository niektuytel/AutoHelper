using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;

namespace AutoHelper.Application.Garages._DTOs;

public class GarageStatisticsDtoItem
{
    public int TotalApprovedServiceLogs { get; set; }
    public int TotalDeniedServiceLogs { get; set; }
    public int TotalPendingServiceLogs { get; set; }
    public ServiceLogsChartPoint[] ServiceLogsChartPoints { get; set; }

    public int TotalProvidedServices { get; set; }

}

public class ServiceLogsChartPoint
{
    DateTime Date { get; set; }
    public int ApprovedAmount { get; set; }
    public int DeniedAmount { get; set; }
    public int PendingAmount { get; set; }
}