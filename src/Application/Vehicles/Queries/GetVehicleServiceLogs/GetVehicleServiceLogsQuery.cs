using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Garages;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;

/// <summary>
/// Only returns service and repair logs
/// </summary>
public record GetVehicleServiceLogsQuery : IRequest<VehicleServiceLogDtoItem[]>
{
    public GetVehicleServiceLogsQuery(string licensePlate)
    {
        LicensePlate = licensePlate;
    }

    public string LicensePlate { get; set; }
}

public class GetVehicleServiceLogsQueryHandler : IRequestHandler<GetVehicleServiceLogsQuery, VehicleServiceLogDtoItem[]>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetVehicleServiceLogsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<VehicleServiceLogDtoItem[]> Handle(GetVehicleServiceLogsQuery request, CancellationToken cancellationToken)
    {
        var entities = _context.VehicleServiceLogs
            .AsNoTracking()
            .Where(v => 
                v.VehicleLicensePlate == request.LicensePlate && (
                    v.Type == GarageServiceType.Service ||
                    v.Type == GarageServiceType.Repair
                )
            )
            .OrderByDescending(v => v.Date);

        var result = await _mapper
            .ProjectTo<VehicleServiceLogDtoItem>(entities)
            .ToArrayAsync(cancellationToken);

        return result;
    }

}
