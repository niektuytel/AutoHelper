using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Queries.GetGarageServiceTypesByLicensePlate;
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
    private readonly IVehicleService _vehicleService;
    private readonly IGarageService _garageInfoService;

    public GetGarageServiceTypesByLicensePlateQueryHandler(IVehicleService vehicleService, IGarageService garageService)
    {
        _vehicleService = vehicleService;
        _garageInfoService = garageService;
    }

    public async Task<IEnumerable<GarageServiceType>> Handle(GetGarageServiceTypesByLicensePlateQuery request, CancellationToken cancellationToken)
    {
        var type = await _vehicleService.GetVehicleType(request.LicensePlate);
        var serviceTypes = _garageInfoService.GetRelatedServiceTypes(type);
        return serviceTypes;
    }

}
