using System;
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

public class GetVehicleDefectsQuery: IRequest<VehicleDefectItem[]>
{
    public GetVehicleDefectsQuery(string licensePlate)
    {
        LicensePlate = licensePlate;
    }

    public string LicensePlate { get; private set; }
}

public class GetVehicleDefectsQueryHandler : IRequestHandler<GetVehicleDefectsQuery, VehicleDefectItem[]>
{
    private readonly IVehicleService _vehicleService;

    public GetVehicleDefectsQueryHandler(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    public async Task<VehicleDefectItem[]> Handle(GetVehicleDefectsQuery request, CancellationToken cancellationToken)
    {
        var info = await _vehicleService.GetVehicleDefectsHistory(request.LicensePlate);
        if (info == null)
        {
            throw new NotFoundException(nameof(VehicleBriefDtoItem), request.LicensePlate);
        }


        return null;
    }
}
