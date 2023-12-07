using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Application.Garages._DTOs;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Application.Garages.Queries.GetGarageSettings;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AutoHelper.Application.Vehicles.Commands.CreateVehicleServiceLogAsGarage;

public record CreateVehicleServiceLogAsGarageCommand : IRequest<VehicleServiceLogAsGarageDtoItem>
{
    public CreateVehicleServiceLogAsGarageCommand(string userId, CreateVehicleServiceAsGarageLogDtoItem data) 
    {
        UserId = userId;
        VehicleLicensePlate = data.VehicleLicensePlate;
        GarageServiceId = data.GarageServiceId;
        Description = data.Description;

        Date = data.Date;
        ExpectedNextDate = data.ExpectedNextDate;
        OdometerReading = data.OdometerReading;
        ExpectedNextOdometerReading = data.ExpectedNextOdometerReading;
    }

    /// <summary>
    /// internal use only, not required for client
    /// </summary>
    [JsonIgnore]
    internal GarageItem Garage { get; set; }

    [JsonIgnore]
    public string UserId { get; set; } = null!;

    public string VehicleLicensePlate { get; set; }
    public Guid? GarageServiceId { get; set; } = null!;
    public string? Description { get; set; }

    public string Date { get; set; }
    public string? ExpectedNextDate { get; set; } = null!;
    public int OdometerReading { get; set; }
    public int? ExpectedNextOdometerReading { get; set; } = null!;

    public VehicleServiceLogAttachmentDtoItem? Attachment { get; set; }

    [JsonIgnore]
    internal DateTime? ParsedDate { get; private set; }

    [JsonIgnore]
    internal DateTime? ParsedExpectedNextDate { get; private set; }

    [JsonIgnore]
    public GarageServiceItem? GarageService { get; internal set; }

    public void SetParsedDates(DateTime? date, DateTime? expectedNextDate)
    {
        ParsedDate = date;
        ParsedExpectedNextDate = expectedNextDate;
    }
}

public class CreateVehicleServiceLogAsGarageCommandHandler : IRequestHandler<CreateVehicleServiceLogAsGarageCommand, VehicleServiceLogAsGarageDtoItem>
{
    private readonly IBlobStorageService _blobStorageService;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IVehicleService _vehicleService;

    public CreateVehicleServiceLogAsGarageCommandHandler(IBlobStorageService blobStorageService, IApplicationDbContext context, IMapper mapper, IVehicleService vehicleService)
    {
        _blobStorageService = blobStorageService;
        _context = context;
        _mapper = mapper;
        _vehicleService = vehicleService;
    }

    public async Task<VehicleServiceLogAsGarageDtoItem> Handle(CreateVehicleServiceLogAsGarageCommand request, CancellationToken cancellationToken)
    {
        var entity = CreateVehicleServiceLogEntity(request);
        UploadAttachmentIfPresent(request, entity, cancellationToken);

        _context.VehicleServiceLogs.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        //entity.AddDomainEvent(new SomeDomainEvent(entity));

        var item = _vehicleService.CreateServiceLogTimelineItem(request.VehicleLicensePlate, entity);
        _context.VehicleTimelineItems.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
        //entity.AddDomainEvent(new SomeDomainEvent(entity));

        return _mapper.Map<VehicleServiceLogAsGarageDtoItem>(entity);
    }

    private VehicleServiceLogItem CreateVehicleServiceLogEntity(CreateVehicleServiceLogAsGarageCommand request)
    {
        var description = request.Description;
        if (string.IsNullOrEmpty(description))
        {
            description = request.Description;
        }

        return new VehicleServiceLogItem
        {
            GarageLookupIdentifier = request.Garage.GarageLookupIdentifier,
            VehicleLicensePlate = request.VehicleLicensePlate,
            Type = request.GarageService!.Type,
            Title = request.GarageService.Title,
            Description = description,
            Status = VehicleServiceLogStatus.VerifiedByGarage,

            Date = (DateTime)request.ParsedDate!,
            ExpectedNextDate = request.ParsedExpectedNextDate,
            OdometerReading = request.OdometerReading,
            ExpectedNextOdometerReading = request.ExpectedNextOdometerReading,
            
            ReporterName = request.Garage.Lookup.Name,
            ReporterPhoneNumber = request.Garage.Lookup.PhoneNumber,
            ReporterEmailAddress = request.Garage.Lookup.EmailAddress,
        };
    }

    private async void UploadAttachmentIfPresent(CreateVehicleServiceLogAsGarageCommand request, VehicleServiceLogItem entity, CancellationToken cancellationToken)
    {
        if (request.Attachment?.FileName != null && request.Attachment?.FileData != null)
        {
            var fileExtension = Path.GetExtension(request.Attachment.FileName);
            var attachmentBlobName = await _blobStorageService.UploadVehicleAttachmentAsync(
                request.Attachment.FileData,
                fileExtension,
                cancellationToken
            );

            entity.AttachedFile = attachmentBlobName;
        }
    }

}
