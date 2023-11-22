using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Queries.GetVehicleBriefInfo;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Vehicles.Queries.GetVehicleTimeline;

public record GetVehicleTimelineQuery : IRequest<VehicleTimelineDtoItem[]>
{
    public GetVehicleTimelineQuery(string licensePlate, int take)
    {
        LicensePlate = licensePlate;
        Take = take;
    }

    public string LicensePlate { get; private set; }
    public int Take { get; private set; }
}

public class GetVehicleTimelineQueryHandler : IRequestHandler<GetVehicleTimelineQuery, VehicleTimelineDtoItem[]>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IVehicleService _vehicleService;


    public GetVehicleTimelineQueryHandler(IApplicationDbContext context, IMapper mapper, IVehicleService vehicleService)
    {
        _context = context;
        _mapper = mapper;
        _vehicleService = vehicleService;
    }

    public async Task<VehicleTimelineDtoItem[]> Handle(GetVehicleTimelineQuery request, CancellationToken cancellationToken)
    {
        var licensePlate = request.LicensePlate.ToUpper().Replace(" ", "").Replace("-", "");

        var entities = _context.VehicleTimelineItems
            .AsNoTracking()
            .Where(x => x.VehicleLicensePlate == licensePlate);

        if(request.Take > 0)
        {
            entities = entities.Take(request.Take);
        }

        var result = await _mapper
            .ProjectTo<VehicleTimelineDtoItem>(entities)
            .ToArrayAsync(cancellationToken);

        return result;
    }


}
