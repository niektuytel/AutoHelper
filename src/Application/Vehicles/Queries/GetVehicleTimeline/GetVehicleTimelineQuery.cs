using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Queries.GetVehicleSpecificationsCard;
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

    public string LicensePlate { get; set; }
    public int Take { get; private set; }
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
        var query = _context.VehicleTimelineItems
            .AsNoTracking()
            .Where(x => x.VehicleLicensePlate == request.LicensePlate)
            .OrderByDescending(x => x.Date)
            .AsQueryable();

        if(request.Take > 0)
        {
            query = query.Take(request.Take);
        }

        var result = await _mapper
            .ProjectTo<VehicleTimelineDtoItem>(query)
            .ToArrayAsync(cancellationToken);

        return result;
    }


}
