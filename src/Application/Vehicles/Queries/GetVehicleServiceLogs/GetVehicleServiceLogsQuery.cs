using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain.Entities.Garages;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;

/// <summary>
/// Only returns service and repair logs
/// </summary>
public record GetVehicleServiceLogsQuery : IRequest<VehicleServiceLogCardDtoItem[]>
{
    public GetVehicleServiceLogsQuery(string licensePlate)
    {
        LicensePlate = licensePlate;
    }

    public string LicensePlate { get; set; }
}

public class GetVehicleServiceLogsQueryHandler : IRequestHandler<GetVehicleServiceLogsQuery, VehicleServiceLogCardDtoItem[]>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetVehicleServiceLogsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<VehicleServiceLogCardDtoItem[]> Handle(GetVehicleServiceLogsQuery request, CancellationToken cancellationToken)
    {
        // INFO: before was like this,
        // As inspection is not a service or repair, it should not be included
        // As when the user insert it and do not show it make it a bit confusing
        //v.VehicleLicensePlate == request.LicensePlate && (
        //    v.Type == GarageServiceType.Service ||
        //    v.Type == GarageServiceType.Repair
        //)

        var entities = _context.VehicleServiceLogs
            .AsNoTracking()
            .Where(v =>
                v.VehicleLicensePlate == request.LicensePlate &&
                v.Type != GarageServiceType.Other

            )
            .OrderByDescending(v => v.Date);

        var result = await _mapper
            .ProjectTo<VehicleServiceLogCardDtoItem>(entities)
            .ToArrayAsync(cancellationToken);

        return result;
    }

}
