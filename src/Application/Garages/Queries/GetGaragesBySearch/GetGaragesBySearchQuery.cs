using System;
using System.Collections.Generic;
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
namespace AutoHelper.Application.Garages.Queries.GetGaragesBySearch;

public record GetGaragesBySearchQuery : IRequest<PaginatedList<GarageItemSearchDto>>
{
    public GetGaragesBySearchQuery(string licensePlate, float latitude, float longitude, int inKmRange = 10, int pageNumber=1, int pageSize=10)
    {
        LicensePlate = licensePlate;
        Latitude = latitude;
        Longitude = longitude;
        InKmRange = inKmRange;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public string LicensePlate { get; private set; }
    public float Latitude { get; private set; }
    public float Longitude { get; private set; }
    public int InKmRange { get; private set; }
    public int PageNumber { get; private set; }
    public int PageSize { get; private set; }
}

public class GetGaragesBySearchQueryHandler : IRequestHandler<GetGaragesBySearchQuery, PaginatedList<GarageItemSearchDto>>
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

    public async Task<PaginatedList<GarageItemSearchDto>> Handle(GetGaragesBySearchQuery request, CancellationToken cancellationToken)
    {
        _context.SetQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

        // First, retrieve potential results from the database without the distance filter
        var potentialResults = await _context.Garages
            .Include(x => x.Location)
            .Include(x => x.Employees)
                .ThenInclude(x => x.WorkExperiences)
            .Include(x => x.Employees)
                .ThenInclude(x => x.WorkSchema)
            .Where(x => x.Employees.Any(y => y.IsActive))
            .ToListAsync();

        // Now, filter and project in-memory using LINQ to Objects
        var filteredResults = potentialResults
            .Select(item => new GarageItemSearchDto()
            {
                Id = item.Id,
                Name = item.Name,
                DistanceInKm = _garageInfoService.CalculateDistanceInKm(
                    item.Location.Latitude,
                    item.Location.Longitude,
                    request.Latitude,
                    request.Longitude
                ),
                Location = item.Location,
                Employees = item.Employees
                    .Where(x => x.IsActive)
                    .Select(e => new GarageEmployeeItemSearchDto()
                    {
                        WorkExperiences = e.WorkExperiences,
                        WorkingDaysOfWeek = e.WorkSchema
                            .Select(x => x.DayOfWeek)
                            .OrderBy(day => day)
                            .Distinct()
                            .ToArray()
                    })
                    .ToList()
            })
            .Where(g => g.DistanceInKm <= request.InKmRange)
            .OrderBy(g => g.DistanceInKm)
            .ToList();

        // Paginate results
        var items = filteredResults
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var pagedResults = new PaginatedList<GarageItemSearchDto>(
            items,
            filteredResults.Count,
            request.PageNumber,
            request.PageSize
        );

        return pagedResults;
    }

}
