using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Domain.Entities.Garages;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Vehicles.Queries.GetVehicleRelatedServices;

public record GetVehicleRelatedServicesQuery : IRequest<IEnumerable<GarageServiceType>>
{
    public GetVehicleRelatedServicesQuery(string licensePlate)
    {
        LicensePlate = licensePlate;
    }

    public string LicensePlate { get; internal set; }
}

public class GetGarageServiceTypesByLicensePlateQueryHandler : IRequestHandler<GetVehicleRelatedServicesQuery, IEnumerable<GarageServiceType>>
{
    private readonly IVehicleService _vehicleService;
    private readonly IGarageService _garageInfoService;

    public GetGarageServiceTypesByLicensePlateQueryHandler(IVehicleService vehicleService, IGarageService garageService)
    {
        _vehicleService = vehicleService;
        _garageInfoService = garageService;
    }

    public async Task<IEnumerable<GarageServiceType>> Handle(GetVehicleRelatedServicesQuery request, CancellationToken cancellationToken)
    {
        // Used to get vehicle specific filters on the frontend
        return new List<GarageServiceType>() { GarageServiceType.Service, GarageServiceType.Repair, GarageServiceType.Inspection };
    }

}
