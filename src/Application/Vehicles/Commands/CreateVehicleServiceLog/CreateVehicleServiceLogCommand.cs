﻿using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Interfaces.Queue;
using AutoHelper.Application.Garages._DTOs;
using AutoHelper.Application.Garages.Commands.CreateGarageReviewNotifier;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;

namespace AutoHelper.Application.Vehicles.Commands.CreateVehicleServiceLog;

public record CreateVehicleServiceLogCommand : IRequest<VehicleServiceLogDtoItem>
{
    public CreateVehicleServiceLogCommand(CreateVehicleServiceLogDtoItem data)
    {
        VehicleLicensePlate = data.VehicleLicensePlate;
        GarageLookupIdentifier = data.GarageLookupIdentifier;
        GarageServiceId = data.GarageServiceId;
        Description = data.Description;

        Date = data.Date;
        ExpectedNextDate = data.ExpectedNextDate;
        OdometerReading = data.OdometerReading;
        ExpectedNextOdometerReading = data.ExpectedNextOdometerReading;

        ReporterName = data.ReporterName;
        ReporterPhoneNumber = data.ReporterPhoneNumber;
        ReporterEmailAddress = data.ReporterEmailAddress;
    }

    public string VehicleLicensePlate { get; set; }
    public string GarageLookupIdentifier { get; set; }

    public Guid? GarageServiceId { get; set; } = null!;
    public string? Description { get; set; }

    public string Date { get; set; }
    public string? ExpectedNextDate { get; set; } = null!;
    public int OdometerReading { get; set; }
    public int? ExpectedNextOdometerReading { get; set; } = null!;

    public string ReporterName { get; set; } = null!;
    public string? ReporterPhoneNumber { get; set; } = null!;
    public string? ReporterEmailAddress { get; set; } = null!;

    public VehicleServiceLogAttachmentDtoItem Attachment { get; set; }

    [JsonIgnore]
    internal DateTime? ParsedDate { get; private set; }

    [JsonIgnore]
    internal DateTime? ParsedExpectedNextDate { get; private set; }

    [JsonIgnore]
    public GarageLookupItem? Garage { get; internal set; }

    [JsonIgnore]
    public GarageServiceDtoItem? GarageService { get; internal set; }

    public void SetParsedDates(DateTime? date, DateTime? expectedNextDate)
    {
        ParsedDate = date;
        ParsedExpectedNextDate = expectedNextDate;
    }
}

public class CreateVehicleServiceLogCommandHandler : IRequestHandler<CreateVehicleServiceLogCommand, VehicleServiceLogDtoItem>
{
    private readonly IBlobStorageService _blobStorageService;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ISender _sender;
    private readonly IQueueService _queueService;
    private readonly IIdentificationHelper _identificationHelper;

    public CreateVehicleServiceLogCommandHandler(
        IBlobStorageService blobStorageService,
        IApplicationDbContext context,
        IMapper mapper,
        ISender sender,
        IQueueService queueService,
        IIdentificationHelper identificationHelper
    )
    {
        _blobStorageService = blobStorageService;
        _context = context;
        _mapper = mapper;
        _sender = sender;
        _queueService = queueService;
        _identificationHelper = identificationHelper;
    }

    public async Task<VehicleServiceLogDtoItem> Handle(CreateVehicleServiceLogCommand request, CancellationToken cancellationToken)
    {
        var entity = CreateVehicleServiceLogEntity(request);
        UploadAttachmentIfPresent(request, entity, cancellationToken);

        _context.VehicleServiceLogs.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        //entity.AddDomainEvent(new SomeDomainEvent(entity));

        // send notification to garage
        var title = $"Ask to review service logs on garage {entity.GarageLookup!.Name}({entity.GarageLookup!.Identifier})";
        var command = new SendGarageServiceReviewCommand(
            entity.VehicleLicensePlate,
            entity.GarageLookupIdentifier,
            entity.Id,
            request.GarageService!.Description!
        );
        _queueService.Enqueue("default", title, command);

        return _mapper.Map<VehicleServiceLogDtoItem>(entity);
    }

    private VehicleServiceLogItem CreateVehicleServiceLogEntity(CreateVehicleServiceLogCommand request)
    {
        return new VehicleServiceLogItem
        {
            GarageLookupIdentifier = request.GarageLookupIdentifier,
            VehicleLicensePlate = request.VehicleLicensePlate,
            Type = request.GarageService!.Type,
            Title = request.GarageService.Title,
            Description = request.Description,

            Date = (DateTime)request.ParsedDate!,
            ExpectedNextDate = request.ParsedExpectedNextDate,
            OdometerReading = request.OdometerReading,
            ExpectedNextOdometerReading = request.ExpectedNextOdometerReading,

            Status = VehicleServiceLogStatus.NotVerified,
            ReporterName = request.ReporterName,
            ReporterPhoneNumber = request.ReporterPhoneNumber,
            ReporterEmailAddress = request.ReporterEmailAddress
        };
    }

    private async void UploadAttachmentIfPresent(CreateVehicleServiceLogCommand request, VehicleServiceLogItem entity, CancellationToken cancellationToken)
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
