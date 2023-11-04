﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles.Queries.GetVehicleBriefInfo;
using AutoHelper.Domain.Entities.Vehicles;
using MediatR;

namespace AutoHelper.Application.Vehicles.Queries.GetVehicleDefects;

public class GetVehicleMOTHistoryQuery: IRequest<VehicleDefectItem[]>
{
    public GetVehicleMOTHistoryQuery(string licensePlate)
    {
        LicensePlate = licensePlate;
    }

    public string LicensePlate { get; private set; }
}

public class GetVehicleDefectsQueryHandler : IRequestHandler<GetVehicleMOTHistoryQuery, VehicleDefectItem[]>
{
    private readonly IVehicleService _vehicleService;

    public GetVehicleDefectsQueryHandler(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    public async Task<VehicleDefectItem[]> Handle(GetVehicleMOTHistoryQuery request, CancellationToken cancellationToken)
    {
        var info = await _vehicleService.GetDefectHistoryByLicensePlateAsync(request.LicensePlate);
        if (info == null)
        {
            throw new NotFoundException(nameof(VehicleBriefDtoItem), request.LicensePlate);
        }


        return null;
    }
}
