using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles._DTOs;
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
            .Where(x => x.VehicleLicensePlate == request.LicensePlate);

        if (request.Take > 0)
        {
            query = query
                .OrderByDescending(x => x.Date)
                .ThenBy(x => x.Type)// Bring MOT succeeded above MOT failed
                .Take(request.Take);
        }
        else
        {
            query = query
                .OrderByDescending(x => x.Date)
                .ThenBy(x => x.Type);// Bring MOT succeeded above MOT failed
        }

        var result = await _mapper
            .ProjectTo<VehicleTimelineDtoItem>(query)
            .ToArrayAsync(cancellationToken);

        return result;
    }


}
