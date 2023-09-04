using System.ComponentModel.DataAnnotations;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Application.Garages.Models;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Deprecated;
using AutoHelper.Domain.Events;
using AutoMapper;
using MediatR;

namespace AutoHelper.Application.Garages.Commands.UpdateGarageItemSettings;


public record UpdateGarageItemSettingsCommand : IRequest<GarageSettings>
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string PhoneNumber { get; set; }

    public string WhatsAppNumber { get; set; } = "";

    public string Email { get; set; }

    public GarageLocationItem Location { get; set; } = new GarageLocationItem();

    public GarageBankingDetailsItem BankingDetails { get; set; } = new GarageBankingDetailsItem();

    public GarageServicesSettingsItem ServicesSettings { get; set; } = new GarageServicesSettingsItem();
}


public class UpdateGarageItemSettingsCommandHandler : IRequestHandler<UpdateGarageItemSettingsCommand, GarageSettings>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateGarageItemSettingsCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GarageSettings> Handle(UpdateGarageItemSettingsCommand request, CancellationToken cancellationToken)
    {
        var entity = new GarageItem
        {
            Id = request.Id,
            Name = request.Name,
            PhoneNumber = request.PhoneNumber,
            WhatsAppNumber = request.WhatsAppNumber,
            Email = request.Email,
            Location = request.Location,
            BankingDetails = request.BankingDetails,
            ServicesSettings = request.ServicesSettings
        };

        // If you wish to use domain events, then you can add them here:
        // entity.AddDomainEvent(new SomeDomainEvent(entity));

        _context.Garages.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<GarageSettings>(entity);
    }
}
