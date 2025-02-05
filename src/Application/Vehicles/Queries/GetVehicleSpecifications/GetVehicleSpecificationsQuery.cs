﻿using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles._DTOs;
using MediatR;

namespace AutoHelper.Application.Vehicles.Queries.GetVehicleSpecifications;

public record GetVehicleSpecificationsQuery : IRequest<VehicleSpecificationsDtoItem>
{
    public GetVehicleSpecificationsQuery(string licensePlate)
    {
        LicensePlate = licensePlate;
    }

    public string LicensePlate { get; set; }
}

public class GetVehicleSpecificationsQueryHandler : IRequestHandler<GetVehicleSpecificationsQuery, VehicleSpecificationsDtoItem>
{
    private readonly IVehicleService _vehicleService;

    public GetVehicleSpecificationsQueryHandler(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    public async Task<VehicleSpecificationsDtoItem> Handle(GetVehicleSpecificationsQuery request, CancellationToken cancellationToken)
    {
        var info = await _vehicleService.GetSpecificationsByLicensePlateAsync(request.LicensePlate);
        if (info == null)
        {
            throw new NotFoundException(nameof(VehicleSpecificationsDtoItem), request.LicensePlate);
        }

        return info;
    }

}
