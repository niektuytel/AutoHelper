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
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Newtonsoft.Json.Linq;

namespace AutoHelper.Application.Garages.Queries.GetGarageLookup;

public record GetGarageLookupQuery : IRequest<GarageLookupDto>
{
    public GetGarageLookupQuery(string identifier, string? licensePlate = null)
    {
        Identifier = identifier;
        LicensePlate = licensePlate;
    }

    public string Identifier { get; private set; }

    public string? LicensePlate { get; private set; }
}

public class GetGaragesBySearchQueryHandler : IRequestHandler<GetGarageLookupQuery, GarageLookupDto>
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

    public async Task<GarageLookupDto> Handle(GetGarageLookupQuery request, CancellationToken cancellationToken)
    {
        var lookup = await _context.GarageLookups
            .Include(x => x.LargeData)// TODO: remove this line, customer not allowed to look at data comming from google and rdw
            .FirstOrDefaultAsync(x => x.Identifier == request.Identifier);

        if (lookup == null)
        {
            throw new NotFoundException(nameof(GarageLookupDto), request.Identifier);
        }

        var response = _mapper.Map<GarageLookupDto>(lookup);
        return response;
    }

}
