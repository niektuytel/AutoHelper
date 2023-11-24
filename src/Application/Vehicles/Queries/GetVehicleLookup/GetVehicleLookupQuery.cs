using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles.Queries.GetVehicleSpecificationsCard;
using AutoHelper.Domain.Entities.Vehicles;
using MediatR;

namespace AutoHelper.Application.Vehicles.Queries.GetVehicleLookup;

public record GetVehicleLookupQuery : IRequest<VehicleLookupItem>
{
    public GetVehicleLookupQuery(string licensePlate)
    {
        LicensePlate = licensePlate;
    }

    public string LicensePlate { get; set; }
}

public class GetVehicleLookupQueryHandler : IRequestHandler<GetVehicleLookupQuery, VehicleLookupItem>
{
    private readonly IApplicationDbContext _context;

    public GetVehicleLookupQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VehicleLookupItem> Handle(GetVehicleLookupQuery request, CancellationToken cancellationToken)
    {
        var entity = _context.VehicleLookups.FirstOrDefault(x => x.LicensePlate == request.LicensePlate);
        if (entity == null)
        {
            throw new NotFoundException(nameof(VehicleLookupItem), request.LicensePlate);
        }

        return entity;
    }


}
