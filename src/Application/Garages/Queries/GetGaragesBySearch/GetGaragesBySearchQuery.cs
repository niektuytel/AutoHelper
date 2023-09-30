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
        // 1. Query the database
        _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        var garages = await _context.Garages
            .Include(x => x.Location)
            .Include(x => x.Employees)
                .ThenInclude(x => x.WorkExperiences)
            .Include(x => x.Employees)
                .ThenInclude(x => x.WorkSchema)
            .Where(x => x.Employees.Any(y => y.IsActive))
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
                            .Distinct()
                            .ToArray()
                    })
                    .ToList()
            })
            .AsSplitQuery() // Split the query
            .ToListAsync();

        // 2. Filter and paginate in memory
        var garagesInRange = garages
            .Where(g => g.DistanceInKm <= request.InKmRange)
            .OrderBy(g => g.DistanceInKm)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return garagesInRange;

        //_context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        //_context.ChangeTracker.LazyLoadingEnabled = false;
        //// 1. Optional: Bounding Box Optimization
        //// Define a method to get the bounding box for latitude and longitude. You can then use these values to filter the garages.

        //// 2. Fetch Relevant Data, that is active
        //var garages = _context.Garages
        //    .Include(x => x.Employees)
        //    .Where(x => x.Employees.Any(y => y.IsActive))
        //    .Include(x => x.Location)
        //    .Include(x => x.Employees)
        //    .ThenInclude(x => x.WorkExperiences)
        //    .Include(x => x.Employees)
        //    .ThenInclude(x => x.WorkSchema)
        //    .Select(item => new GarageItemSearchDto()
        //    {
        //        Id = item.Id,
        //        Name = item.Name,
        //        DistanceInKm = _garageInfoService.CalculateDistanceInKm(
        //            item.Location.Latitude,
        //            item.Location.Longitude,
        //            request.Latitude,
        //            request.Longitude
        //        ),
        //        Location = item.Location,
        //        Employees = item.Employees
        //            .Where(x => x.IsActive)
        //            .Select(e => new GarageEmployeeItemSearchDto()
        //            {
        //                WorkExperiences = e.WorkExperiences,
        //                WorkingDaysOfWeek = e.WorkSchema
        //                    .Select(x => x.DayOfWeek)
        //                    .Distinct()
        //                    .ToArray()
        //            })
        //            .ToList()
        //    })
        //    .ToList();

        //// 3. Filter in memory
        //var garagesInRange = await garages
        //    .AsQueryable()
        //    .Where(g => g.DistanceInKm <= request.InKmRange)
        //    .OrderBy(g => g.DistanceInKm)
        //    .PaginatedListAsync(request.PageNumber, request.PageSize);

        //////.Where(g => g.DistanceInKm <= request.InKmRange)
        ////.OrderBy(g => g.DistanceInKm)
        ////.PaginatedListAsync(request.PageNumber, request.PageSize);

        //// 3. Filter in memory
        ////var garagesInRange = garages.ToList();


        ////.PaginatedListAsync(request.PageNumber, request.PageSize);


        return null;// garagesInRange;
    }

}
