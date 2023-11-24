using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.HPRtree;

namespace AutoHelper.Application.Vehicles.Commands.UpsertVehicleLookupByReporter;

public class UpsertVehicleLookupByReporterCommand : IRequest<VehicleLookupDtoItem>
{
    public UpsertVehicleLookupByReporterCommand(
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
        ReporterPhoneNumber = phoneNumber;
        ReportWhatsappNumber = whatsappNumber;
        ReporterEmailAddress = emailAddress;
    }

    [Required]
    public string LicensePlate { get; set; } = null!;

    public DateTime? MOTExpiryDate { get; internal set; }

    public DateTime? DateOfAscription { get; internal set; }

    [Required]
    public string Longitude { get; set; } = null!;
    [Required]
    public string Latitude { get; set; } = null!;
    public Point Location { get; internal set; }

    public string? ReporterPhoneNumber { get; set; } = null!;

    public string? ReportWhatsappNumber { get; set; } = null!;

    public string? ReporterEmailAddress { get; set; } = null!;

}

public class UpsertVehicleLookupByReporterCommandHandler : IRequestHandler<UpsertVehicleLookupByReporterCommand, VehicleLookupDtoItem>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IVehicleService _vehicleInfoService;

    public UpsertVehicleLookupByReporterCommandHandler(IApplicationDbContext context, IMapper mapper, IVehicleService vehicleInfoService)
    {
        _context = context;
        _mapper = mapper;
        _vehicleInfoService = vehicleInfoService;
    }
    public async Task<VehicleLookupDtoItem> Handle(UpsertVehicleLookupByReporterCommand request, CancellationToken cancellationToken)
    {
        var entity = _context.VehicleLookups.FirstOrDefault(v => v.LicensePlate == request.LicensePlate);
        if (entity == null)
        {

            entity = new VehicleLookupItem()
            {
                LicensePlate = request.LicensePlate,
                DateOfMOTExpiry = request.MOTExpiryDate,
                DateOfAscription = request.DateOfAscription,
                Location = request.Location,
                ReporterPhoneNumber = request.ReporterPhoneNumber,
                ReporterWhatsappNumber = request.ReportWhatsappNumber,
                ReporterEmailAddress = request.ReporterEmailAddress
            };

            _context.VehicleLookups.Add(entity);
        }
        else
        {
            entity.DateOfMOTExpiry = request.MOTExpiryDate;
            entity.DateOfAscription = request.DateOfAscription;
            entity.Location = request.Location;
            entity.ReporterPhoneNumber = request.ReporterPhoneNumber;
            entity.ReporterWhatsappNumber = request.ReportWhatsappNumber;
            entity.ReporterEmailAddress = request.ReporterEmailAddress;

            _context.VehicleLookups.Update(entity);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<VehicleLookupDtoItem>(entity);
    }

}
