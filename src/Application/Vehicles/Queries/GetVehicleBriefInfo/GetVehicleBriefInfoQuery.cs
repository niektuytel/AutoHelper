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

public record GetVehicleBriefInfoQuery : IRequest<VehicleBriefInfoItemDto>
{
    public GetVehicleBriefInfoQuery(string licensePlate)
    {
        LicensePlate = licensePlate;
    }

    public string LicensePlate { get; private set; }
}

public class GetVehicleBriefInfoQueryHandler : IRequestHandler<GetVehicleBriefInfoQuery, VehicleBriefInfoItemDto>
{
    private readonly IVehicleInfoService _vehicleService;

    public GetVehicleBriefInfoQueryHandler(IVehicleInfoService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    public async Task<VehicleBriefInfoItemDto> Handle(GetVehicleBriefInfoQuery request, CancellationToken cancellationToken)
    {
        var info = await _vehicleService.GetVehicleBriefInfo(request.LicensePlate);
        if (info == null)
        {
            throw new NotFoundException(nameof(VehicleBriefInfoItemDto), request.LicensePlate);
        }

        return info;
    }


}
