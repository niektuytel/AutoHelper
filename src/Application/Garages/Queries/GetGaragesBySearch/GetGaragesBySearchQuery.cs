using System;
using System.Collections.Generic;
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
        // 1. Optional: Bounding Box Optimization
        // Define a method to get the bounding box for latitude and longitude. You can then use these values to filter the garages.

        // 2. Fetch Relevant Data
        var garages = await _context.Garages
            .Include(x => x.Location)
            .Include(x => x.Employees)
            //.ThenInclude(x => x.WorkExperiences)
            //.Include(x => x.Employees)
            //.ThenInclude(x => x.WorkSchema)
            .ToListAsync();

        // 3. Filter In Memory (for ordering)
        var filteredGarages = garages
            .Where(x =>
                x.Employees.Any(y =>
                    y.IsActive &&
                    y.WorkExperiences.Any() &&
                    y.WorkSchema.Any()
                )
            )
            .Where(g => _garageInfoService.CalculateDistanceInKm(g.Location, request.Latitude, request.Longitude) <= request.InKmRange)
            .OrderBy(g => _garageInfoService.CalculateDistanceInKm(g.Location, request.Latitude, request.Longitude))
            .ToList();

        // 4. Use ProjectTo and PaginatedListAsync
        var garagesInRange = await filteredGarages
            .AsQueryable() // Convert the list back to IQueryable
            .ProjectTo<GarageItemSearchDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);


        return garagesInRange;
    }

}
