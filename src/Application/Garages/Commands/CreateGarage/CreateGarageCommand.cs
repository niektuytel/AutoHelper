using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Application.Garages._DTOs;
using AutoHelper.Application.Garages.Queries.GetGarageSettings;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Garages;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.HPRtree;
using NetTopologySuite;

namespace AutoHelper.Application.Garages.Commands.CreateGarageItem;

public record CreateGarageCommand : IRequest<GarageSettingsDtoItem>
{
    [JsonIgnore]
    public string? UserId { get; set; }

    [JsonIgnore]
    public GarageLookupItem GarageLookup { get; set; } = new GarageLookupItem();

    public string GarageLookupIdentifier { get; set; } = "";

    public string? Website { get; set; }

    public string? PhoneNumber { get; set; }

    public string? WhatsappNumber { get; set; } 

    public string? EmailAddress { get; set; }

    public string? ConversationEmail { get; set; }

    public string? ConversationWhatsappNumber { get; set; }

    public GarageLocationDtoItem Location { get; set; }

}

public class CreateGarageItemCommandHandler : IRequestHandler<CreateGarageCommand, GarageSettingsDtoItem>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateGarageItemCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GarageSettingsDtoItem> Handle(CreateGarageCommand request, CancellationToken cancellationToken)
    {
        // Create garage
        var entity = new GarageItem
        {
            UserId = request.UserId!,
            GarageLookupIdentifier = request.GarageLookupIdentifier
        };

        _context.Garages.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        // Insert all services
        foreach (var serviceType in request.GarageLookup.KnownServices)
        {
            if(serviceType == GarageServiceType.Other) continue;

            var service = new GarageServiceItem
            {
                UserId = request.UserId!,
                GarageId = entity.Id,
                Type = serviceType
            };

            _context.GarageServices.Add(service);
        }
        await _context.SaveChangesAsync(cancellationToken);

        // Update lookup
        request.GarageLookup.GarageId = entity.Id;
        request.GarageLookup.Website = request.Website;
        request.GarageLookup.PhoneNumber = request.PhoneNumber;
        request.GarageLookup.WhatsappNumber = request.WhatsappNumber;
        request.GarageLookup.EmailAddress = request.EmailAddress;
        request.GarageLookup.ConversationContactEmail = request.ConversationEmail;
        request.GarageLookup.ConversationContactWhatsappNumber = request.ConversationWhatsappNumber;

        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        request.GarageLookup.Location = geometryFactory.CreatePoint(new Coordinate(request.Location.Longitude, request.Location.Latitude));
        request.GarageLookup.Address = request.Location.Address;
        request.GarageLookup.City = request.Location.City;

        request.GarageLookup.LastModifiedBy = $"{nameof(CreateGarageCommand)}:{request.UserId}";
        request.GarageLookup.LastModified = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        // If you wish to use domain events, then you can add them here:
        // entity.AddDomainEvent(new SomeDomainEvent(entity));

        return _mapper.Map<GarageSettingsDtoItem>(entity);
    }
}
