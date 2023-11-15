using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;
using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Application.Garages.Queries.GetGarageSettings;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Vehicles.Commands.CreateVehicleServiceLog;

public record CreateVehicleServiceLogWithAttachmentDto : IRequest<VehicleServiceLogItemDto>
{
    public CreateVehicleServiceLogCommand ServiceLogCommand { get; set; }
    public IFormFile AttachmentFile { get; set; }
}

public record CreateVehicleServiceLogCommand : IRequest<VehicleServiceLogItemDto>
{
    public string VehicleLicensePlate { get; set; }

    public string PerformedByGarageName { get; set; }

    public GarageServiceType Type { get; set; } = GarageServiceType.Other;

    public string? Description { get; set; }


    public DateTime Date { get; set; }

    public DateTime? ExpectedNextDate { get; set; } = null!;

    public int OdometerReading { get; set; }

    public int? ExpectedNextOdometerReading { get; set; } = null!;

    public VehicleServiceLogAttachmentItemOnCreateDto Attachment { get; set; }

}

public class CreateVehicleServiceLogCommandHandler : IRequestHandler<CreateVehicleServiceLogCommand, VehicleServiceLogItemDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateVehicleServiceLogCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<VehicleServiceLogItemDto> Handle(CreateVehicleServiceLogCommand request, CancellationToken cancellationToken)
    {
        var entity = new VehicleServiceLogItem
        {
            //UserId = request.UserId,
            //Name = request.Name,
            //PhoneNumber = request.PhoneNumber,
            //WhatsAppNumber = request.WhatsAppNumber,
            //Email = request.Email,
            //Location = new GarageLocationItem
            //{
            //    Address = request.Location.Address,
            //    PostalCode = request.Location.PostalCode,
            //    City = request.Location.City,
            //    Country = request.Location.Country,
            //    Longitude = request.Location.Longitude,
            //    Latitude = request.Location.Latitude
            //},
            //BankingDetails = new GarageBankingDetailsItem
            //{
            //    BankName = request.BankingDetails.BankName,
            //    KvKNumber = request.BankingDetails.KvKNumber,
            //    AccountHolderName = request.BankingDetails.AccountHolderName,
            //    IBAN = request.BankingDetails.IBAN
            //}
        };

        //// If you wish to use domain events, then you can add them here:
        //// entity.AddDomainEvent(new SomeDomainEvent(entity));

        //_context.Garages.Add(entity);
        //await _context.SaveChangesAsync(cancellationToken);
        return null;
    }
}
