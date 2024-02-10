using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Models;
using AutoHelper.Application.Garages._DTOs;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace AutoHelper.Application.Garages.Queries.GetGarageLookups;

public record GetGarageLookupsQuery : IRequest<PaginatedList<GarageLookupBriefDto>>
{
    public GetGarageLookupsQuery(
        string licensePlate,
        float latitude,
        float longitude,
        int inMeterRange = 10,
        string? autoCompleteOnGarageName = null,
        string[]? filters = null,
        int pageNumber = 1,
        int pageSize = 10
    )
    {
        LicensePlate = licensePlate;
        Latitude = latitude;
        Longitude = longitude;
        InMeterRange = inMeterRange;
        AutoCompleteOnGarageName = autoCompleteOnGarageName;
        Filters = filters;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public string LicensePlate { get; internal set; }
    public float Latitude { get; private set; }
    public float Longitude { get; private set; }
    public int InMeterRange { get; private set; }
    public string? AutoCompleteOnGarageName { get; private set; }
    public string[]? Filters { get; private set; }
    public int PageNumber { get; private set; }
    public int PageSize { get; private set; }

    [JsonIgnore]
    public VehicleType? VehicleType { get; internal set; }
}

public class GetGaragesBySearchQueryHandler : IRequestHandler<GetGarageLookupsQuery, PaginatedList<GarageLookupBriefDto>>
{
    private readonly IVehicleService _vehicleService;
    private readonly IGarageService _garageInfoService;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetGaragesBySearchQueryHandler(IVehicleService vehicleInfoService, IGarageService garageInfoService, IApplicationDbContext context, IMapper mapper)
    {
        _vehicleService = vehicleInfoService;
        _garageInfoService = garageInfoService;
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<GarageLookupBriefDto>> Handle(GetGarageLookupsQuery request, CancellationToken cancellationToken)
    {
        // contains location, days of week, services
        var queryable = _context.GarageLookups
            .AsNoTracking()
            .Include(x => x.Services)
            .Where(x => x.Services.Any(a => a.VehicleType == request.VehicleType))
            .Where(x => x.Location != null)
            .Where(x => EF.Functions.Like(x.DaysOfWeekString, "%–%"))
            .Where(x => x.Services.Any())
        ;

        queryable = WhenHasRelatedGarageName(queryable, request.AutoCompleteOnGarageName);
        queryable = await WhenHasSelectedFilters(queryable, request.Filters);

        // (filter + order by) distance
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        var userLocation = geometryFactory.CreatePoint(new Coordinate(request.Longitude, request.Latitude));
        queryable = queryable
            .Where(g => g.Location!.Distance(userLocation) <= request.InMeterRange)
            .OrderBy(x => x.Location!.Distance(userLocation));

        // paginating the results.
        var totalRecords = queryable.Count();
        var pageRecords = queryable
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(item => new GarageLookupBriefDto(item, item.Location!.Distance(userLocation), _mapper))
            .ToList();

        // in-memory getting related services
        foreach (var item in pageRecords)
        {
            item.Services = GetRelatedServices(item);
        }

        var pagedResults = new PaginatedList<GarageLookupBriefDto>(
            pageRecords,
            totalRecords,
            request.PageNumber,
            request.PageSize
        );

        return pagedResults;
    }

    private IQueryable<GarageLookupItem> WhenHasRelatedGarageName(IQueryable<GarageLookupItem> queryable, string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return queryable;
        }

        value = value.ToLower();
        queryable = queryable.Where(x => x.Name.ToLower().Contains(value));

        return queryable;
    }

    private async Task<IQueryable<GarageLookupItem>> WhenHasSelectedFilters(IQueryable<GarageLookupItem> queryable, string[]? filters)
    {
        // Remove any null values from the filters array
        filters = filters?.Where(f => f != null).ToArray();
        if (filters?.Any() != true)
        {
            return queryable;
        }

        // All filters should match for the item to be included
        foreach (var filter in filters)
        {
            queryable = queryable.Where(x =>
                x.Services.Any(y => ((int)y.Type).ToString() == filter)
            );
        }

        return queryable;
    }

    private IEnumerable<GarageServiceDtoItem> GetRelatedServices(GarageLookupBriefDto request)
    {
        if (request.GarageId != null)
        {
            var entities = _context.GarageServices.Where(x => x.GarageId == request.GarageId);
            return _mapper.Map<IEnumerable<GarageServiceDtoItem>>(entities) ?? new List<GarageServiceDtoItem>();
        }

        return request.Services;
    }
}
