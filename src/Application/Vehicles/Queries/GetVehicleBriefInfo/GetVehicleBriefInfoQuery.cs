using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using AutoHelper.Application.WeatherForecasts.Queries.GetWeatherForecasts;
using AutoHelper.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Vehicles.Queries.GetVehicleBriefInfo;

public record GetVehicleBriefInfoQuery : IRequest<VehicleBriefInfoItem>
{
    public GetVehicleBriefInfoQuery(string licensePlate)
    {
        LicensePlate = licensePlate;
    }

    public string LicensePlate { get; private set; }
}

public class GetVehicleBriefInfoQueryHandler : IRequestHandler<GetVehicleBriefInfoQuery, VehicleBriefInfoItem>
{
    private readonly IVehicleService _vehicleService;

    public GetVehicleBriefInfoQueryHandler(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    public async Task<VehicleBriefInfoItem> Handle(GetVehicleBriefInfoQuery request, CancellationToken cancellationToken)
    {
        var info = await _vehicleService.GetVehicleBriefInfo(request.LicensePlate);
        if (info == null)
        {
            throw new NotFoundException(nameof(VehicleBriefInfoItem), request.LicensePlate);
        }

        return info;
    }


}
