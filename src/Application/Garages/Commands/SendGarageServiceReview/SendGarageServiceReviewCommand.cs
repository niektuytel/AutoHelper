using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;
using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Application.Messages.Commands.CreateNotificationMessage;
using AutoHelper.Application.Garages._DTOs;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Application.Garages.Queries.GetGarageSettings;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Commands.CreateVehicleServiceLogAsGarage;
using AutoHelper.Domain;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Messages.Enums;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoHelper.Hangfire.Shared.MediatR;
using AutoHelper.Application.Messages.Commands.SendNotificationMessage;
using Hangfire;
using AutoHelper.Hangfire.Shared.Interfaces;

namespace AutoHelper.Application.Garages.Commands.CreateGarageReviewNotifier;

public record SendGarageServiceReviewCommand : IQueueRequest<Unit>
{
    public SendGarageServiceReviewCommand(string licensePlate, string garageIdentifier, Guid serviceLogId, string description)
    {
        LicensePlate = licensePlate;
        GarageIdentifier = garageIdentifier;
        ServiceLogId = serviceLogId;
        Description = description;
    }

    public string LicensePlate { get; set; }
    public string GarageIdentifier { get; set; }
    public Guid ServiceLogId { get; set; }
    public string Description { get; set; }

    [JsonIgnore]
    public GarageLookupItem? Garage { get; internal set; }

    [JsonIgnore]
    public IQueueService QueueingService { get; set; }

}

public class CreateGarageServiceReviewNotifierCommandHandler : IRequestHandler<SendGarageServiceReviewCommand, Unit>
{
    private readonly IBlobStorageService _blobStorageService;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ISender _sender;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IIdentificationHelper _identificationHelper;

    public CreateGarageServiceReviewNotifierCommandHandler(
        IBlobStorageService blobStorageService,
        IApplicationDbContext context,
        IMapper mapper,
        ISender sender,
        IBackgroundJobClient backgroundJobClient,
        IIdentificationHelper identificationHelper
    )
    {
        _blobStorageService = blobStorageService;
        _context = context;
        _mapper = mapper;
        _sender = sender;
        _backgroundJobClient = backgroundJobClient;
        _identificationHelper = identificationHelper;
    }

    public async Task<Unit> Handle(SendGarageServiceReviewCommand request, CancellationToken cancellationToken)
    {
        var metaData = new Dictionary<string, string>
        {
            { "desciption", request.Description },
            { "serviceLogId", request.ServiceLogId.ToString()},
            { "garageIdentifier", request.Garage!.Identifier },
        };

        // send notification to garage
        var emailAddress = request.Garage!.ConversationContactEmail;
        if (string.IsNullOrWhiteSpace(emailAddress))
        {
            emailAddress = request.Garage.EmailAddress;
        }

        var whatappNumber = request.Garage.ConversationContactWhatsappNumber;
        if (string.IsNullOrWhiteSpace(whatappNumber))
        {
            whatappNumber = request.Garage.WhatsappNumber;
        }

        var contactIdentifier = _identificationHelper.GetValidIdentifier(emailAddress, whatappNumber);
        await SendNotificationToGarage(request.LicensePlate, contactIdentifier, metaData, cancellationToken);

        return Unit.Value;
    }

    private async Task SendNotificationToGarage(string licencePlate, string contactIdentifier, Dictionary<string, string> metadata, CancellationToken cancellationToken)
    {
        var notificationCommand = new CreateNotificationCommand(
            licencePlate,
            GeneralNotificationType.GarageServiceReviewReminder,
            VehicleNotificationType.Other,
            triggerDate: null,
            contactIdentifier: contactIdentifier,
            metadata: metadata
        );
        var notification = await _sender.Send(notificationCommand, cancellationToken);

        // schedule notification
        var queue = nameof(SendNotificationMessageCommand);
        var schuduleCommand = new SendNotificationMessageCommand(notification.Id);
        var title = $"{notificationCommand.VehicleLicensePlate}_{notification.GeneralType.ToString()}";
        _sender.Enqueue(_backgroundJobClient, queue, title, schuduleCommand);
    }

}
