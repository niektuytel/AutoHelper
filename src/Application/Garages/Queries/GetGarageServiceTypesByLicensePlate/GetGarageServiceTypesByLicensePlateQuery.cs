using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Queries.GetGarageServiceTypesByLicensePlate;
using AutoHelper.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using AutoHelper.Application.WeatherForecasts.Queries.GetWeatherForecasts;
using AutoHelper.Domain.Entities.Garages;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Queries.GetGarageServiceTypesByLicensePlate;

public record GetGarageServiceTypesByLicensePlateQuery : IRequest<IEnumerable<GarageServiceType>>
{
    public GetGarageServiceTypesByLicensePlateQuery(string licensePlate)
    {
        LicensePlate = licensePlate;
    }

    public string LicensePlate { get; private set; }
}

public class GetGarageServiceTypesByLicensePlateQueryHandler : IRequestHandler<GetGarageServiceTypesByLicensePlateQuery, IEnumerable<GarageServiceType>>
{
    private readonly IVehicleInfoService _vehicleInfoService;

    public GetGarageServiceTypesByLicensePlateQueryHandler(IVehicleInfoService vehicleInfoService)
    {
        _vehicleInfoService = vehicleInfoService;
    }

    public async Task<IEnumerable<GarageServiceType>> Handle(GetGarageServiceTypesByLicensePlateQuery request, CancellationToken cancellationToken)
    {
        var type = await _vehicleInfoService.GetVehicleType(request.LicensePlate);
        return type switch
        {
            Domain.Entities.Vehicles.VehicleType.LightCar => new List<GarageServiceType>()
            {
                GarageServiceType.MOTServiceLightVehicle
            },
            Domain.Entities.Vehicles.VehicleType.HeavyCar => new List<GarageServiceType>()
            {
                GarageServiceType.MOTServiceHeavyVehicle
            },
            _ => new List<GarageServiceType>(),
        };
    }

}
