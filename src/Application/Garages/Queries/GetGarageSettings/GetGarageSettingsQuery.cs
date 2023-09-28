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

namespace AutoHelper.Application.Garages.Queries.GetGarageSettings;

public record GetGarageSettingsQuery : IRequest<GarageItemDto>
{
    public GetGarageSettingsQuery(string userId)
    {
        UserId = userId;
    }

    public string UserId { get; set; }
}

public class GetGarageSettingsQueryHandler : IRequestHandler<GetGarageSettingsQuery, GarageItemDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetGarageSettingsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GarageItemDto> Handle(GetGarageSettingsQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Garages
            .Include(g => g.Location)
            .Include(g => g.BankingDetails)
            .FirstOrDefaultAsync(x => x.UserId == request.UserId);

        if (entity == null)
        {
            throw new NotFoundException(nameof(GarageItem), request.UserId);
        }

        return _mapper.Map<GarageItemDto>(entity);
    }

}
