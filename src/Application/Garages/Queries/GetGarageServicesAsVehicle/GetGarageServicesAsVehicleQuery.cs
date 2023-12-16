using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages._DTOs;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain.Entities.Garages;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Queries.GetGarageServicesAsVehicle;

public record GetGarageServicesAsVehicleQuery : IRequest<IEnumerable<GarageServiceDtoItem>>
{
    public GetGarageServicesAsVehicleQuery(string garageLookupIdentifier, string? licensePlate=null)
    {
        LicensePlate = licensePlate;
        GarageLookupIdentifier = garageLookupIdentifier;
    }

    public string? LicensePlate { get; internal set; }

    public string GarageLookupIdentifier { get; internal set; }

    [JsonIgnore]
    public GarageLookupItem? GarageLookup { get; internal set; }
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
        // TODO: get services on vehicle type, to reduce response size

        IEnumerable<GarageServiceDtoItem> result;
        if (request.GarageLookup!.GarageId != null)
        {
            var entities = _context.GarageServices
                .Where(x => x.GarageId == request.GarageLookup.GarageId);

            result = _mapper.Map<IEnumerable<GarageServiceDtoItem>>(entities) ?? new List<GarageServiceDtoItem>();
        }
        else
        {
            var entities = _context.GarageLookupServices
                .Where(x => x.GarageLookupIdentifier == request.GarageLookupIdentifier);

            result = _mapper.Map<IEnumerable<GarageServiceDtoItem>>(entities) ?? new List<GarageServiceDtoItem>();
        }

        return result;
    }

}
