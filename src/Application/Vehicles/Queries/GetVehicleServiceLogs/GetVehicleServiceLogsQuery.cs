using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Queries.GetGarageEmployees;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;

public record GetVehicleServiceLogsQuery : IRequest<VehicleServiceLogItemDto[]>
{
    public GetVehicleServiceLogsQuery(string licensePlate)
    {
        LicensePlate = licensePlate;
    }

    public string LicensePlate { get; private set; }
}

public class GetVehicleServiceLogsQueryHandler : IRequestHandler<GetVehicleServiceLogsQuery, VehicleServiceLogItemDto[]>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetVehicleServiceLogsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<VehicleServiceLogItemDto[]> Handle(GetVehicleServiceLogsQuery request, CancellationToken cancellationToken)
    {
        var licensePlate = request.LicensePlate.ToUpper().Replace(" ", "").Replace("-", "");

        var entities = _context.VehicleServiceLogs
            .AsNoTracking()
            .Where(v => v.VehicleLicensePlate == licensePlate);

        var result = await _mapper
            .ProjectTo<VehicleServiceLogItemDto>(entities)
            .ToArrayAsync(cancellationToken);

        return result;
    }

}
