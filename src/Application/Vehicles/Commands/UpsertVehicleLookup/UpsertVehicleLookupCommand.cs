using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Conversations.Commands.StartConversation;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;
using NetTopologySuite.Geometries;

namespace AutoHelper.Application.Vehicles.Commands.UpsertVehicleLookup;

public class UpsertVehicleLookupCommand : IRequest<VehicleLookupDtoItem>
{
    public UpsertVehicleLookupCommand(
        string licensePlate,
        string latitude,
        string longitude,
        string? phoneNumber,
        string? whatsappNumber,
        string? emailAddress
    )
    {
        LicensePlate = licensePlate;
        Latitude = latitude;
        Longitude = longitude;
        PhoneNumber = phoneNumber;
        WhatsappNumber = whatsappNumber;
        EmailAddress = emailAddress;
    }

    [Required]
    public string LicensePlate { get; set; } = null!;

    public DateTime MOTExpiryDate { get; internal set; }

    [Required]
    public string Longitude { get; set; } = null!;
    [Required]
    public string Latitude { get; set; } = null!;
    public Point Location { get; internal set; }

    public string? PhoneNumber { get; set; } = null!;

    public string? WhatsappNumber { get; set; } = null!;

    public string? EmailAddress { get; set; } = null!;

}

public class UpsertVehicleLookupCommandHandler : IRequestHandler<UpsertVehicleLookupCommand, VehicleLookupDtoItem>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IVehicleInfoService _vehicleInfoService;

    public UpsertVehicleLookupCommandHandler(IApplicationDbContext context, IMapper mapper, IVehicleInfoService vehicleInfoService)
    {
        _context = context;
        _mapper = mapper;
        _vehicleInfoService = vehicleInfoService;
    }
    public async Task<VehicleLookupDtoItem> Handle(UpsertVehicleLookupCommand request, CancellationToken cancellationToken)
    {
        var entity = _context.VehicleLookups.FirstOrDefault(v => v.LicensePlate == request.LicensePlate);
        if (entity == null)
        {
            entity = new VehicleLookupItem()
            {
                LicensePlate = request.LicensePlate,
                MOTExpiryDate = request.MOTExpiryDate,
                Location = request.Location,
                PhoneNumber = request.PhoneNumber,
                WhatsappNumber = request.WhatsappNumber,
                EmailAddress = request.EmailAddress
            };

            _context.VehicleLookups.Add(entity);
        }
        else
        {
            entity.MOTExpiryDate = request.MOTExpiryDate,
            entity.Location = request.Location,
            entity.PhoneNumber = request.PhoneNumber;
            entity.WhatsappNumber = request.WhatsappNumber;
            entity.EmailAddress = request.EmailAddress;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<VehicleLookupDtoItem>(entity);
    }

}
