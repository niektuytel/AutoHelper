using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Application.Common.Models;
using AutoHelper.Application.Garages._DTOs;
using AutoHelper.Domain.Entities.Garages;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Newtonsoft.Json.Linq;

namespace AutoHelper.Application.Garages.Queries.GetGarageLookup;

public record GetGarageLookupQuery : IRequest<GarageLookupDtoItem>
{
    public GetGarageLookupQuery(string garageLookupIdentifier, string? licensePlate = null)
    {
        GarageLookupIdentifier = garageLookupIdentifier;
        LicensePlate = licensePlate;
    }

    public string? LicensePlate { get; internal set; }

    public string GarageLookupIdentifier { get; internal set; }

    [JsonIgnore]
    public GarageLookupItem? GarageLookup { get; internal set; }
}

public class GetGaragesBySearchQueryHandler : IRequestHandler<GetGarageLookupQuery, GarageLookupDtoItem>
{
    private readonly IVehicleService _vehicleInfoService;
    private readonly IGarageService _garageInfoService;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetGaragesBySearchQueryHandler(IVehicleService vehicleInfoService, IGarageService garageInfoService, IApplicationDbContext context, IMapper mapper)
    {
        _vehicleInfoService = vehicleInfoService;
        _garageInfoService = garageInfoService;
        _context = context;
        _mapper = mapper;
    }

    public async Task<GarageLookupDtoItem> Handle(GetGarageLookupQuery request, CancellationToken cancellationToken)
    {
        var response = _mapper.Map<GarageLookupDtoItem>(request.GarageLookup);
        response.Services = UpdateGarageServices(request);
        response.Services.OrderBy(x => x.VehicleType);

        return response;
    }

    private IEnumerable<GarageServiceDtoItem> UpdateGarageServices(GetGarageLookupQuery request)
    {
        if (request.GarageLookup!.GarageId != null)
        {
            var entities = _context.GarageServices
                .Where(x => x.GarageId == request.GarageLookup.GarageId);
            return _mapper.Map<IEnumerable<GarageServiceDtoItem>>(entities) ?? new List<GarageServiceDtoItem>();
        }

        return _mapper.Map<IEnumerable<GarageServiceDtoItem>>(request.GarageLookup.Services) ?? new List<GarageServiceDtoItem>();
    }

}
