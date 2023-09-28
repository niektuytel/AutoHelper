using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Application.Garages.Queries.GetGarageSettings;
using AutoHelper.Domain.Entities.Deprecated;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoHelper.Domain.Events;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Commands.UpdateGarageItemSettings;


public record UpdateGarageSettingsCommand : IRequest<GarageItem>
{
    public string Name { get; set; }

    public string PhoneNumber { get; set; }

    public string WhatsAppNumber { get; set; } = "";

    public string Email { get; set; }

    public GarageLocationItem Location { get; set; } = new GarageLocationItem();

    public GarageBankingDetailsItem BankingDetails { get; set; } = new GarageBankingDetailsItem();

    public GarageServicesSettingsItem ServicesSettings { get; set; } = new GarageServicesSettingsItem();

    [JsonIgnore]
    public string UserId { get; set; }

}

public class UpdateGarageItemSettingsCommandHandler : IRequestHandler<UpdateGarageSettingsCommand, GarageItem>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateGarageItemSettingsCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GarageItem> Handle(UpdateGarageSettingsCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Garages
            .Include(x => x.Location)
            .Include(x => x.BankingDetails)
            .FirstOrDefaultAsync(x => x.UserId == request.UserId, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException($"{nameof(GarageItem)} on UserId:", request.UserId);
        }

        entity.Name = request.Name;
        entity.PhoneNumber = request.PhoneNumber;
        entity.WhatsAppNumber = request.WhatsAppNumber;
        entity.Email = request.Email;
        entity.Location.Address = request.Location.Address;
        entity.Location.City = request.Location.City;
        entity.Location.PostalCode = request.Location.PostalCode;
        entity.Location.Country = request.Location.Country;
        entity.Location.Longitude = request.Location.Longitude;
        entity.Location.Latitude = request.Location.Latitude;
        entity.BankingDetails.BankName = request.BankingDetails.BankName;
        entity.BankingDetails.KvKNumber = request.BankingDetails.KvKNumber;
        entity.BankingDetails.AccountHolderName = request.BankingDetails.AccountHolderName;
        entity.BankingDetails.IBAN = request.BankingDetails.IBAN;

        // If you wish to use domain events, then you can add them here:
        // entity.AddDomainEvent(new SomeDomainEvent(entity));

        // Since we fetched the entity directly from the DbContext, it's already tracked. 
        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }
}
