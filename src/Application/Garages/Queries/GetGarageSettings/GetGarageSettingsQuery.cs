using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Models;
using AutoHelper.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using AutoHelper.Application.WeatherForecasts.Queries.GetWeatherForecasts;
using AutoHelper.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Queries.GetGarageSettings;

public record GetGarageSettingsQuery : IRequest<GarageSettings>
{
    public GetGarageSettingsQuery(Guid garageId)
    {
        GarageId = garageId;
    }

    public Guid GarageId { get; set; }
}

public class GetGarageSettingsQueryHandler : IRequestHandler<GetGarageSettingsQuery, GarageSettings>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetGarageSettingsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GarageSettings> Handle(GetGarageSettingsQuery request, CancellationToken cancellationToken)
    {
        var garageEntity = await _context.Garages
            .Include(g => g.Location)
            .Include(g => g.BankingDetails)
            .Include(g => g.Contacts)
            .FirstOrDefaultAsync(x => x.Id == request.GarageId);

        if (garageEntity == null)
        {
            throw new NotFoundException(nameof(GarageItem), request.GarageId);
        }

        return _mapper.Map<GarageSettings>(garageEntity);
    }

}
