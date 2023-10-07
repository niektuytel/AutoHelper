using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Application.Common.Models;
using AutoHelper.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using AutoHelper.Application.WeatherForecasts.Queries.GetWeatherForecasts;
using AutoHelper.Domain.Entities.Garages;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace AutoHelper.Application.Garages.Queries.GetGaragesLookups;

public record GetGarageLookupsQuery : IRequest<PaginatedList<GarageLookupDto>>
{
    public GetGarageLookupsQuery(
        string licensePlate,
        float latitude,
        float longitude,
        int inKmRange = 10,
        string? autoCompleteOnGarageName = null,
        int pageNumber = 1,
        int pageSize = 10
    )
    {
        LicensePlate = licensePlate;
        Latitude = latitude;
        Longitude = longitude;
        InKmRange = inKmRange;
        AutoCompleteOnGarageName = autoCompleteOnGarageName;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public string LicensePlate { get; private set; }
    public float Latitude { get; private set; }
    public float Longitude { get; private set; }
    public int InKmRange { get; private set; }
    public string? AutoCompleteOnGarageName { get; private set; }
    public int PageNumber { get; private set; }
    public int PageSize { get; private set; }
}

public class GetGaragesBySearchQueryHandler : IRequestHandler<GetGarageLookupsQuery, PaginatedList<GarageLookupDto>>
{
    private readonly IGarageInfoService _garageInfoService;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetGaragesBySearchQueryHandler(IGarageInfoService garageInfoService, IApplicationDbContext context, IMapper mapper)
    {
        _garageInfoService = garageInfoService;
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<GarageLookupDto>> Handle(GetGarageLookupsQuery request, CancellationToken cancellationToken)
    {
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        var userLocation = geometryFactory.CreatePoint(new Coordinate(request.Longitude, request.Latitude));

        var queryable = _context.GarageLookups
            .AsNoTracking()
            .Where(x => x.Location != null
                        && !string.IsNullOrEmpty(x.KnownServicesString)
                        && !string.IsNullOrEmpty(x.DaysOfWeekString)
                        && (!string.IsNullOrEmpty(x.Website) || x.GarageId != null));

        if (!string.IsNullOrEmpty(request.AutoCompleteOnGarageName))
        {
            var value = request.AutoCompleteOnGarageName.ToLower();
            queryable = queryable.Where(x => x.Name.ToLower().Contains(value));
        }

        var filteredResults = queryable
            .OrderBy(x => x.Location!.Distance(userLocation))
            .Select(item => new GarageLookupDto(item, item.Location!.Distance(userLocation)))
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();
            //.Where(g => g.DistanceInKm <= request.InKmRange)

        var pagedResults = new PaginatedList<GarageLookupDto>(
            filteredResults,
            filteredResults.Count,
            request.PageNumber,
            request.PageSize
        );

        return pagedResults;
    }
}
