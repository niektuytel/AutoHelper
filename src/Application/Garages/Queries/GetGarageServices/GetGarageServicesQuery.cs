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

namespace AutoHelper.Application.Garages.Queries.GetGarageServices;

public record GetGarageServicesQuery : IRequest<IEnumerable<GarageServiceItem>>
{
    public GetGarageServicesQuery(Guid garageId)
    {
        GarageId = garageId;
    }

    public Guid GarageId { get; set; }
}

public class GetGarageServicesQueryHandler : IRequestHandler<GetGarageServicesQuery, IEnumerable<GarageServiceItem>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetGarageServicesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<GarageServiceItem>> Handle(GetGarageServicesQuery request, CancellationToken cancellationToken)
    {
        var entities = _context.GarageServices
            .Where(x => x.GarageId == request.GarageId)
            .AsEnumerable();

        return entities ?? new List<GarageServiceItem>();
    }

}
