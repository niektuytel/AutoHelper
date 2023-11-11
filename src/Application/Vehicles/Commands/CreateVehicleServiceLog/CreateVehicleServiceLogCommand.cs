using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Application.Garages.Queries.GetGarageSettings;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Garages;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Vehicles.Commands.CreateVehicleServiceLog;

public record CreateVehicleServiceLogCommand : IRequest<VehicleServiceLogItemDto>
{

}

public class CreateGarageItemCommandHandler : IRequestHandler<CreateVehicleServiceLogCommand, GarageItem>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateGarageItemCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GarageItem> Handle(CreateVehicleServiceLogCommand request, CancellationToken cancellationToken)
    {
        var entity = new GarageItem
        {
            UserId = request.UserId,
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
            }
        };

        // If you wish to use domain events, then you can add them here:
        // entity.AddDomainEvent(new SomeDomainEvent(entity));

        _context.Garages.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }
}
