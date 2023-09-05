using System.ComponentModel.DataAnnotations;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Application.Garages.Models;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Deprecated;
using AutoHelper.Domain.Events;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Commands.CreateGarageItem;

public record CreateGarageCommand : IRequest<GarageSettings>
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string PhoneNumber { get; set; }

    public string WhatsAppNumber { get; set; } = "";

    public string Email { get; set; }

    public BriefLocationDto Location { get; set; }

    public BriefBankingDetailsDto BankingDetails { get; set; }
}

public class CreateGarageItemCommandHandler : IRequestHandler<CreateGarageCommand, GarageSettings>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateGarageItemCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GarageSettings> Handle(CreateGarageCommand request, CancellationToken cancellationToken)
    {
        var entity = new GarageItem
        {
            Id = request.Id,
            Name = request.Name,
            PhoneNumber = request.PhoneNumber,
            WhatsAppNumber = request.WhatsAppNumber,
            Email = request.Email,
            Location = new GarageLocationItem
            {
                Address = request.Location.Address,
                PostalCode = request.Location.PostalCode,
                City = request.Location.City,
                Country = request.Location.Country,
                Longitude = request.Location.Longitude,
                Latitude = request.Location.Latitude
            },
            BankingDetails = new GarageBankingDetailsItem
            {
                BankName = request.BankingDetails.BankName,
                KvKNumber = request.BankingDetails.KvKNumber,
                AccountHolderName = request.BankingDetails.AccountHolderName,
                IBAN = request.BankingDetails.IBAN
            },
            ServicesSettings = new GarageServicesSettingsItem()
        };

        // If you wish to use domain events, then you can add them here:
        // entity.AddDomainEvent(new SomeDomainEvent(entity));

        _context.Garages.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<GarageSettings>(entity);
    }
}
