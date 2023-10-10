using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Domain.Entities.Garages;
using MediatR;

namespace AutoHelper.Application.Vehicles.Queries.GetVehicleRelatedServices;

public record GetVehicleRelatedServicesQuery : IRequest<IEnumerable<GarageServiceType>>
{
    public GetVehicleRelatedServicesQuery(string licensePlate)
    {
        LicensePlate = licensePlate;
    }

    public string LicensePlate { get; private set; }
}

public class GetVehicleRelatedServicesQueryQueryHandler : IRequestHandler<GetVehicleRelatedServicesQuery, IEnumerable<GarageServiceType>>
{
    private readonly IVehicleInfoService _vehicleService;

    public GetVehicleRelatedServicesQueryQueryHandler(IVehicleInfoService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    public async Task<IEnumerable<GarageServiceType>> Handle(GetVehicleRelatedServicesQuery request, CancellationToken cancellationToken)
    {
        var services = await _vehicleService.GetRelatedServiceTypesByLicencePlate(request.LicensePlate);
        if (services == null)
        {
            throw new NotFoundException(nameof(GarageServiceType), request.LicensePlate);
        }

        return services;
    }

}
