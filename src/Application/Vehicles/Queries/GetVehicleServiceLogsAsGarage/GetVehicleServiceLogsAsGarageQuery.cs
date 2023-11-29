using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Garages;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogsAsGarage;

public record GetVehicleServiceLogsAsGarageQuery : IRequest<VehicleServiceLogAsGarageDtoItem[]>
{
    public GetVehicleServiceLogsAsGarageQuery(string userId, string licensePlate)
    {
        UserId = userId;
        LicensePlate = licensePlate;
    }

    [JsonIgnore]
    public string UserId { get; set; } = null!;

    [JsonIgnore]
    public GarageItem Garage { get; set; } = null!;

    public string? LicensePlate { get; set; }
}

public class GetVehicleServiceLogsAsGarageQueryHandler : IRequestHandler<GetVehicleServiceLogsAsGarageQuery, VehicleServiceLogAsGarageDtoItem[]>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetVehicleServiceLogsAsGarageQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<VehicleServiceLogAsGarageDtoItem[]> Handle(GetVehicleServiceLogsAsGarageQuery request, CancellationToken cancellationToken)
    {
        var entities = _context.VehicleServiceLogs
            .AsNoTracking()
            .Where(v => v.GarageLookupIdentifier == request.Garage.GarageLookupIdentifier);

        if(!string.IsNullOrWhiteSpace(request.LicensePlate))
        {
            entities = entities.Where(v => v.VehicleLicensePlate == request.LicensePlate);
        }

        var result = await _mapper
            .ProjectTo<VehicleServiceLogAsGarageDtoItem>(entities)
            .ToArrayAsync(cancellationToken);

        return result;
    }

}
