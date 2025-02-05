﻿using AutoHelper.Application.Vehicles._DTOs;

namespace AutoHelper.Application.Garages._DTOs;

public class GarageOverviewDtoItem
{

    public GarageOverviewDtoItem(
        int totalApprovedServiceLogs,
        int totalPendingServiceLogs,
        int totalServedVehicle,
        ServiceLogsChartPoint[] chartPoints,
        List<VehicleServiceLogAsGarageDtoItem> recentServiceLogs,
        List<GarageServiceDtoItem> supportedServices
    )
    {
        TotalApprovedServiceLogs = totalApprovedServiceLogs;
        TotalPendingServiceLogs = totalPendingServiceLogs;
        TotalServedVehicle = totalServedVehicle;
        ChartPoints = chartPoints;
        RecentServiceLogs = recentServiceLogs;
        SupportedServices = supportedServices;
    }


    public int TotalApprovedServiceLogs { get; set; }
    public int TotalPendingServiceLogs { get; set; }
    public int TotalServedVehicle { get; set; }

    public ServiceLogsChartPoint[] ChartPoints { get; set; }
    public List<VehicleServiceLogAsGarageDtoItem> RecentServiceLogs { get; set; }
    public List<GarageServiceDtoItem> SupportedServices { get; set; }

}

public class ServiceLogsChartPoint
{
    public ServiceLogsChartPoint(int approvedAmount = 0, int pendingAmount = 0, int vehiclesAmount = 0)
    {
        ApprovedAmount = approvedAmount;
        PendingAmount = pendingAmount;
        VehiclesAmount = vehiclesAmount;
    }

    public int ApprovedAmount { get; set; }
    public int PendingAmount { get; set; }
    public int VehiclesAmount { get; set; }
}