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

namespace AutoHelper.Application.Vehicles.Queries.GetVehicleInfo;

public record GetVehicleInfoQuery : IRequest<VehicleInfoItemDto>
{
    public GetVehicleInfoQuery(string licensePlate)
    {
        LicensePlate = licensePlate;
    }

    public string LicensePlate { get; private set; }
}

public class GetVehicleInfoQueryQueryHandler : IRequestHandler<GetVehicleInfoQuery, VehicleInfoItemDto>
{
    private readonly IVehicleService _vehicleService;

    public GetVehicleInfoQueryQueryHandler(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    public async Task<VehicleInfoItemDto> Handle(GetVehicleInfoQuery request, CancellationToken cancellationToken)
    {
        var info = await _vehicleService.GetVehicleInfoQuery(request.LicensePlate);
        if (info == null)
        {
            throw new NotFoundException(nameof(VehicleInfoItemDto), request.LicensePlate);
        }

        return info;
    }

}
