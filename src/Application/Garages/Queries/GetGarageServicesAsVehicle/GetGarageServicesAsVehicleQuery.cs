using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages._DTOs;
using AutoHelper.Domain.Entities.Garages;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Queries.GetGarageServicesAsVehicle;

public record GetGarageServicesAsVehicleQuery : IRequest<IEnumerable<GarageServiceDtoItem>>
{
    public GetGarageServicesAsVehicleQuery(string userId)
    {
        UserId = userId;
    }

    [JsonIgnore]
    public string UserId { get; private set; }

    [JsonIgnore]
    public GarageItem? Garage { get; set; } = new GarageItem();

}

public class GetGarageServicesQueryHandler : IRequestHandler<GetGarageServicesAsVehicleQuery, IEnumerable<GarageServiceDtoItem>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetGarageServicesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<GarageServiceDtoItem>> Handle(GetGarageServicesAsVehicleQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Garages.FirstOrDefaultAsync(x => x.UserId == request.UserId);
        if (entity == null)
        {
            throw new NotFoundException(nameof(GarageItem), request.UserId);
        }

        var entities = _context.GarageServices
            .Where(x =>
                x.UserId == request.UserId &&
                x.GarageId == entity.Id
            )
            .AsEnumerable();

        return _mapper.Map<IEnumerable<GarageServiceDtoItem>>(entities) ?? new List<GarageServiceDtoItem>();
    }

}
