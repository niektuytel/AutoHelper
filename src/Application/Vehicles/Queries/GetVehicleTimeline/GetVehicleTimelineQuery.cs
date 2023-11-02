using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles.Queries.GetVehicleBriefInfo;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Vehicles.Queries.GetVehicleTimeline;

public record GetVehicleTimelineQuery : IRequest<VehicleTimelineDtoItem[]>
{
    public GetVehicleTimelineQuery(string licensePlate)
    {
        LicensePlate = licensePlate;
    }

    public string LicensePlate { get; private set; }
}

public class GetVehicleTimelineQueryHandler : IRequestHandler<GetVehicleTimelineQuery, VehicleTimelineDtoItem[]>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetVehicleTimelineQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<VehicleTimelineDtoItem[]> Handle(GetVehicleTimelineQuery request, CancellationToken cancellationToken)
    {
        var vehicle = await _context.VehicleLookups
            .AsNoTracking()
            .Include(x => x.Timeline)
            .FirstOrDefaultAsync(x => x.LicensePlate == request.LicensePlate);

        if (vehicle == null)
        {
            throw new NotFoundException(nameof(VehicleLookupItem), request.LicensePlate);
        }

        var response = _mapper.Map<VehicleTimelineDtoItem[]>(vehicle.Timeline);
        return response;
    }


}
