﻿using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages._DTOs;
using AutoHelper.Domain.Entities.Garages;
using AutoMapper;
using MediatR;

namespace AutoHelper.Application.Garages.Queries.GetGarageServices;

public record GetGarageServicesQuery : IRequest<IEnumerable<GarageServiceDtoItem>>
{
    public GetGarageServicesQuery(string userId, string? licensePlate = null)
    {
        UserId = userId;
        LicensePlate = licensePlate;
    }

    [JsonIgnore]
    public string UserId { get; private set; }

    [JsonIgnore]
    public GarageItem? Garage { get; set; } = new GarageItem();

    public string? LicensePlate { get; internal set; }

}

public class GetGarageServicesQueryHandler : IRequestHandler<GetGarageServicesQuery, IEnumerable<GarageServiceDtoItem>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetGarageServicesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<GarageServiceDtoItem>> Handle(GetGarageServicesQuery request, CancellationToken cancellationToken)
    {
        var entities = _context.GarageServices
            .Where(x => x.GarageId == request.Garage!.Id);

        var result = _mapper.Map<IEnumerable<GarageServiceDtoItem>>(entities) ?? new List<GarageServiceDtoItem>();
        return result;
    }

}
