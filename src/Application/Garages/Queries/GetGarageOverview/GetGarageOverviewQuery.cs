using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages._DTOs;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain.Entities.Garages;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Queries.GetGarageOverview;

public class GetGarageOverviewQuery : IRequest<GarageOverviewDtoItem>
{
    public GetGarageOverviewQuery(string userId)
    {
        UserId = userId;
    }

    [JsonIgnore]
    public string UserId { get; private set; }

    [JsonIgnore]
    public GarageItem? Garage { get; set; } = new GarageItem();

}

public class GetGarageOverviewQueryHandler : IRequestHandler<GetGarageOverviewQuery, GarageOverviewDtoItem>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetGarageOverviewQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GarageOverviewDtoItem> Handle(GetGarageOverviewQuery request, CancellationToken cancellationToken)
    {
        var query = _context.VehicleServiceLogs.Where(x =>
            x.GarageLookupIdentifier == request.Garage!.GarageLookupIdentifier &&
            x.Status == Domain.VehicleServiceLogStatus.VerifiedByGarage &&
            x.Date >= DateTime.Now.AddYears(-1) // last year - now
        )
        .OrderByDescending(x => x.Date);

        var chartPoints = Enumerable.Range(0, 12)
            .Select(_ => new ServiceLogsChartPoint())
            .ToArray();

        var serviceLogs = query.GroupBy(x => x.Date.Month);
        foreach (var item in serviceLogs)
        {
            var monthIndex = item.Key - 1;

            var approvedCount = item.Where(x => x.Status == Domain.VehicleServiceLogStatus.VerifiedByGarage).Count();
            chartPoints[monthIndex].ApprovedAmount = approvedCount;

            var pendingCount = item.Where(x => x.Status == Domain.VehicleServiceLogStatus.NotVerified).Count();
            chartPoints[monthIndex].PendingAmount = pendingCount;

            var vehiclesCount = item.Select(x => x.VehicleLicensePlate).Distinct().Count();
            chartPoints[monthIndex].VehiclesAmount = vehiclesCount;
        }

        var totalApprovedServiceLogs = chartPoints.Length > 0 ? chartPoints.Sum(x => x.ApprovedAmount) : 0;
        var totalPendingServiceLogs = chartPoints.Length > 0 ? chartPoints.Sum(x => x.PendingAmount) : 0;
        var totalServedVehicles = chartPoints.Length > 0 ? chartPoints.Sum(x => x.VehiclesAmount) : 0;

        var supportedServices = _mapper.Map<List<GarageServiceDtoItem>>(request.Garage!.Services);

        var recentServiceLogs = await query.Take(15).ToListAsync(cancellationToken);
        var recentServiceLogsDto = _mapper.Map<List<VehicleServiceLogAsGarageDtoItem>>(recentServiceLogs);

        var statistics = new GarageOverviewDtoItem(
            totalApprovedServiceLogs,
            totalPendingServiceLogs,
            totalServedVehicles,
            chartPoints,
            recentServiceLogsDto,
            supportedServices
        );

        return statistics;
    }

}