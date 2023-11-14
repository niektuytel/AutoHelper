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
        //var entities = await _context.Vehicles
        //    .AsNoTracking()
        //    .Include(x => x.ServiceLogs)
        //    .ThenInclude(x => x.ServiceItems)
        //    .FirstOrDefaultAsync(x => x.LicensePlate == request.LicensePlate);
        var entity = await _context.VehicleLookups
            .AsNoTracking()
            .Where(v => v.LicensePlate == request.LicensePlate)
            .FirstOrDefaultAsync();


        var result = new VehicleServiceLogItemDto();

        throw new NotImplementedException();

        return null;// _mapper.Map<IEnumerable<GarageEmployeeItemDto>>(entities) ?? new List<GarageEmployeeItemDto>();

        //var info = await _vehicleService.GetVehicleServiceLogsQuery(request.LicensePlate);
        //if (info == null)
        //{
        //    throw new NotFoundException(nameof(VehicleServiceLogsItem), request.LicensePlate);
        //}

        //return info;
    }

}
