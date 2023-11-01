using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
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
    private readonly IApplicationDbContext _context;

    public GetVehicleDefectsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VehicleDefectItem[]> Handle(GetVehicleDefectsQuery request, CancellationToken cancellationToken)
    {
        var entity = _context.VehicleLookups.FirstOrDefault(x => x.LicensePlate == request.LicensePlate);
        if (entity == null)
        {
            throw new NotFoundException(nameof(VehicleLookupItem), request.LicensePlate);
        }

        return null;
    }
}
