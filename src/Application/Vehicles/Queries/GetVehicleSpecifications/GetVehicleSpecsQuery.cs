using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Vehicles.Queries.GetVehicleSpecs;

public record GetVehicleSpecsQuery : IRequest<VehicleSpecsDtoItem>
{
    public GetVehicleSpecsQuery(string licensePlate)
    {
        LicensePlate = licensePlate;
    }

    public string LicensePlate { get; private set; }
}

public class GetVehicleInfoQueryQueryHandler : IRequestHandler<GetVehicleSpecsQuery, VehicleSpecsDtoItem>
{
    private readonly IVehicleService _vehicleService;

    public GetVehicleInfoQueryQueryHandler(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    public async Task<VehicleSpecsDtoItem> Handle(GetVehicleSpecsQuery request, CancellationToken cancellationToken)
    {
        var info = await _vehicleService.GetVehicleInfoQuery(request.LicensePlate);
        if (info == null)
        {
            throw new NotFoundException(nameof(VehicleSpecsDtoItem), request.LicensePlate);
        }

        return info;
    }

}
