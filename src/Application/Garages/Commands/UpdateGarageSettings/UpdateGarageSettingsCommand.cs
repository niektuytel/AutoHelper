using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages._DTOs;
using AutoHelper.Domain.Entities.Garages;
using AutoMapper;
using MediatR;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace AutoHelper.Application.Garages.Commands.UpdateGarageItemSettings;


public record UpdateGarageSettingsCommand : IRequest<GarageSettingsDtoItem>
{
    [JsonIgnore]
    public string? UserId { get; set; }

    [JsonIgnore]
    public GarageItem? Garage { get; set; } = new GarageItem();

    public string? Name { get; set; } = "";

    public string? Website { get; set; }

    public string? PhoneNumber { get; set; }

    public string? WhatsappNumber { get; set; }

    public string? EmailAddress { get; set; }

    public string? ConversationEmail { get; set; }

    public string? ConversationWhatsappNumber { get; set; }

    public GarageLocationDtoItem? Location { get; set; }

}

public class UpdateGarageItemSettingsCommandHandler : IRequestHandler<UpdateGarageSettingsCommand, GarageSettingsDtoItem>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateGarageItemSettingsCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GarageSettingsDtoItem> Handle(UpdateGarageSettingsCommand request, CancellationToken cancellationToken)
    {
        if (request.Garage == null)
        {
            throw new NotFoundException(nameof(GarageItem), request.Name);
        }
        else if (request.Garage.Lookup.GarageId == null)
        {
            // NOTE: check how this can happen anyways? (should on create also add garage ID if so this can been removed)
            request.Garage.Lookup.GarageId = request.Garage.Id;
        }


        request.Garage.Lookup.Name = request.Name;
        //TODO: request.GarageLookup.Image = entity.Image;
        //TODO: request.GarageLookup.ImageThumbnail = entity.ImageThumbnail;
        request.Garage.Lookup.Website = request.Website;
        request.Garage.Lookup.PhoneNumber = request.PhoneNumber;
        request.Garage.Lookup.WhatsappNumber = request.WhatsappNumber;
        request.Garage.Lookup.EmailAddress = request.EmailAddress;
        request.Garage.Lookup.ConversationContactEmail = request.ConversationEmail;
        request.Garage.Lookup.ConversationContactWhatsappNumber = request.ConversationWhatsappNumber;

        if (request.Location != null && request.Location?.Longitude != default && request.Location?.Longitude != default)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            request.Garage.Lookup.Location = geometryFactory.CreatePoint(new Coordinate(request.Location!.Longitude, request.Location!.Latitude));
            request.Garage.Lookup.Address = request.Location.Address;
            request.Garage.Lookup.City = request.Location.City;
        }

        // If you wish to use domain events, then you can add them here:
        // entity.AddDomainEvent(new SomeDomainEvent(entity));

        // Update Garagelookup
        request.Garage.Lookup.LastModifiedBy = $"{nameof(UpdateGarageSettingsCommand)}:{request.UserId}";
        request.Garage.Lookup.LastModified = DateTime.UtcNow;
        _context.GarageLookups.Update(request.Garage.Lookup);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<GarageSettingsDtoItem>(request.Garage);
    }
}
