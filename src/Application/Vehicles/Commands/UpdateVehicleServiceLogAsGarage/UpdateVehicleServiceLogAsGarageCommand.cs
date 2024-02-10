﻿using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Messages.Commands.CreateNotificationMessage;
using AutoHelper.Application.Messages.Commands.SendNotificationMessage;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Commands.CreateVehicleTimeline;
using AutoHelper.Domain;
using AutoHelper.Domain.Entities.Communication;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Vehicles.Commands.UpdateVehicleServiceLogAsGarage;

public record UpdateVehicleServiceLogAsGarageCommand : IRequest<VehicleServiceLogAsGarageDtoItem>
{
    public UpdateVehicleServiceLogAsGarageCommand(string userId, UpdateVehicleServiceAsGarageLogDtoItem data)
    {
        UserId = userId;
        Id = data.Id;
        VehicleLicensePlate = data.VehicleLicensePlate;
        GarageServiceId = data.GarageServiceId;
        Description = data.Description;
        Date = data.Date;
        ExpectedNextDate = data.ExpectedNextDate;
        OdometerReading = data.OdometerReading;
        ExpectedNextOdometerReading = data.ExpectedNextOdometerReading;
        Status = data.Status;
    }

    public Guid Id { get; private set; }
    internal string UserId { get; private set; }

    public string VehicleLicensePlate { get; set; }
    public Guid? GarageServiceId { get; set; } = null!;
    public string? Description { get; set; }
    public VehicleServiceLogStatus Status { get; set; }

    public string Date { get; set; }
    public string? ExpectedNextDate { get; set; } = null!;
    public int OdometerReading { get; set; }
    public int? ExpectedNextOdometerReading { get; set; } = null!;

    public VehicleServiceLogAttachmentDtoItem? Attachment { get; set; }

    internal GarageItem Garage { get; set; }

    internal VehicleServiceLogItem ServiceLog { get; set; } = null!;

    internal DateTime? ParsedDate { get; private set; }

    internal DateTime? ParsedExpectedNextDate { get; private set; }

    public void SetParsedDates(DateTime? date, DateTime? expectedNextDate)
    {
        ParsedDate = date;
        ParsedExpectedNextDate = expectedNextDate;
    }
}

public class UpdateVehicleServiceLogAsGarageCommandHandler : IRequestHandler<UpdateVehicleServiceLogAsGarageCommand, VehicleServiceLogAsGarageDtoItem>
{
    private readonly IBlobStorageService _blobStorageService;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ISender _sender;
    private readonly IQueueService _queueService;
    private readonly IIdentificationHelper _identificationHelper;


    public UpdateVehicleServiceLogAsGarageCommandHandler(
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

    public async Task<VehicleServiceLogAsGarageDtoItem> Handle(UpdateVehicleServiceLogAsGarageCommand request, CancellationToken cancellationToken)
    {
        var serviceLog = await _context.GarageServices
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == request.GarageServiceId,
                cancellationToken: cancellationToken
            );

        var entity = UpdateVehicleServiceLogEntity(request, serviceLog);
        UploadAttachmentIfPresent(request, entity, cancellationToken);

        // update entity
        await _context.SaveChangesAsync(cancellationToken);
        // entity.AddDomainEvent(new SomeDomainEvent(entity));

        // insert timeline + send reporter a notification
        if (entity.Status == VehicleServiceLogStatus.VerifiedByGarage)
        {
            var timelineCommand = new CreateVehicleTimelineCommand(entity);
            var timeline = await _sender.Send(timelineCommand, cancellationToken);

            await SendNotificationToReporter(
                entity.VehicleLicensePlate,
                request.ServiceLog.ReporterEmailAddress!,
                request.ServiceLog.ReporterPhoneNumber!,
                cancellationToken
            );
        }

        return _mapper.Map<VehicleServiceLogAsGarageDtoItem>(entity);
    }

    private VehicleServiceLogItem UpdateVehicleServiceLogEntity(UpdateVehicleServiceLogAsGarageCommand request, GarageServiceItem? serviceItem)
    {
        var description = request.Description;

        var serviceLog = request.ServiceLog;
        serviceLog.VehicleLicensePlate = request.VehicleLicensePlate;
        if (serviceItem != null)
        {
            serviceLog.Type = serviceItem.Type;
            serviceLog.Title = serviceItem.Title;

            if (string.IsNullOrEmpty(description))
            {
                description = serviceItem.Description;
            }
        }
        serviceLog.Description = description;
        serviceLog.Status = request.Status;

        serviceLog.Date = (DateTime)request.ParsedDate!;
        serviceLog.ExpectedNextDate = request.ParsedExpectedNextDate;
        serviceLog.OdometerReading = request.OdometerReading;
        serviceLog.ExpectedNextOdometerReading = request.ExpectedNextOdometerReading;

        return serviceLog;
    }

    private async void UploadAttachmentIfPresent(UpdateVehicleServiceLogAsGarageCommand request, VehicleServiceLogItem entity, CancellationToken cancellationToken)
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

    private async Task SendNotificationToReporter(string licencePlate, string emailAddress, string whatsappNumber, CancellationToken cancellationToken)
    {
        var contactIdentifier = _identificationHelper.GetValidIdentifier(emailAddress, whatsappNumber);
        var notificationCommand = new CreateNotificationCommand(
            licencePlate,
            NotificationGeneralType.VehicleServiceReviewApproved,
            NotificationVehicleType.Other,
            triggerDate: null,
            contactIdentifier: contactIdentifier
        );
        var notification = await _sender.Send(notificationCommand, cancellationToken);

        // schedule notification
        var queue = nameof(SendNotificationMessageCommand);
        var schuduleCommand = new SendNotificationMessageCommand(notification.Id);
        var title = $"{notificationCommand.VehicleLicensePlate}_{notification.GeneralType.ToString()}";
        _queueService.Enqueue(queue, title, schuduleCommand);
    }

}
