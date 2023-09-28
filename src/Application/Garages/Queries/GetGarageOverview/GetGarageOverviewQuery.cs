using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using AutoHelper.Application.WeatherForecasts.Queries.GetWeatherForecasts;
using AutoHelper.Domain.Entities.Garages;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Queries.GetGarageOverview;

public record GetGarageOverviewQuery : IRequest<GarageOverview>
{
    public GetGarageOverviewQuery(string userId)
    {
        UserId = userId;
    }

    public string UserId { get; private set; }
}

public class GetGarageOverviewQueryHandler : IRequestHandler<GetGarageOverviewQuery, GarageOverview>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetGarageOverviewQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GarageOverview> Handle(GetGarageOverviewQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Garages
            .Include(g => g.Vehicles)
            .FirstOrDefaultAsync(x => x.UserId == request.UserId);

        if (entity == null)
        {
            throw new NotFoundException(nameof(GarageItem), request.UserId);
        }

        return _mapper.Map<GarageOverview>(entity);
    }

}
