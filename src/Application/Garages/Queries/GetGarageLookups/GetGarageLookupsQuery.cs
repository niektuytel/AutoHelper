﻿using System;
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
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Newtonsoft.Json.Linq;

namespace AutoHelper.Application.Garages.Queries.GetGaragesLookups;

public record GetGarageLookupsQuery : IRequest<PaginatedList<GarageLookupDto>>
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

    public string LicensePlate { get; private set; }
    public float Latitude { get; private set; }
    public float Longitude { get; private set; }
    public int InMeterRange { get; private set; }
    public string? AutoCompleteOnGarageName { get; private set; }
    public string[]? Filters { get; private set; }
    public int PageNumber { get; private set; }
    public int PageSize { get; private set; }
}

public class GetGaragesBySearchQueryHandler : IRequestHandler<GetGarageLookupsQuery, PaginatedList<GarageLookupDto>>
{
    private readonly IVehicleInfoService _vehicleInfoService;
    private readonly IGarageInfoService _garageInfoService;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetGaragesBySearchQueryHandler(IVehicleInfoService vehicleInfoService, IGarageInfoService garageInfoService, IApplicationDbContext context, IMapper mapper)
    {
        _vehicleInfoService = vehicleInfoService;
        _garageInfoService = garageInfoService;
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<GarageLookupDto>> Handle(GetGarageLookupsQuery request, CancellationToken cancellationToken)
    {
        var queryable = _context.GarageLookups
            .AsNoTracking()
            .Where(x => x.Location != null
                        && !string.IsNullOrEmpty(x.KnownServicesString)
                        && !string.IsNullOrEmpty(x.DaysOfWeekString)
                        && (!string.IsNullOrEmpty(x.Website) || x.GarageId != null)
            );

        queryable = WhenHasRelatedGarageName(queryable, request.AutoCompleteOnGarageName);
        queryable = await WhenHasSelectedFilters(queryable, request.LicensePlate, request.Filters);

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
            .Select(item => new GarageLookupDto(item, item.Location!.Distance(userLocation)))
            .ToList();

        var pagedResults = new PaginatedList<GarageLookupDto>(
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
    private async Task<IQueryable<GarageLookupItem>> WhenHasSelectedFilters(IQueryable<GarageLookupItem> queryable, string? licensePlate, string[]? filters)
    {
        bool filtersFromLicensePlate = false;

        // set vehicle related filters if not set by user.
        if (filters?.Any() != true && !string.IsNullOrEmpty(licensePlate))
        {
            var type = await _vehicleInfoService.GetVehicleType(licensePlate);
            filters = _garageInfoService.GetRelatedServiceTypes(type).Select(x => ((int)x).ToString()).ToArray();
            filtersFromLicensePlate = true;
        }

        // Remove any null values from the filters array
        filters = filters?.Where(f => f != null).ToArray();

        if (filters?.Any() != true)
        {
            return queryable;
        }

        if (filtersFromLicensePlate)
        {
            var predicate = PredicateBuilder.New<GarageLookupItem>(false);

            foreach (var filter in filters)
            {
                string currentFilter = filter; // To capture the current filter in closure
                predicate = predicate.Or(x => x.KnownServicesString.Contains(currentFilter));
            }

            queryable = queryable.AsExpandable().Where(predicate);

        }
        else
        {
            // All filters should match for the item to be included
            foreach (var filter in filters)
            {
                queryable = queryable.Where(x => x.KnownServicesString.Contains(filter));
            }
        }

        return queryable;
    }


}
