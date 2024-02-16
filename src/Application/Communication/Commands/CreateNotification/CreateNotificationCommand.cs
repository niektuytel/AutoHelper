using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Extensions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Domain.Entities.Communication;
using AutoHelper.Domain.Entities.Messages;
using AutoHelper.Domain.Entities.Vehicles;
using MediatR;

namespace AutoHelper.Application.Messages.Commands.CreateNotificationMessage;

public record CreateNotificationCommand : IRequest<NotificationItem>
{

    public CreateNotificationCommand()
    {

    }

    public CreateNotificationCommand(
        string vehicleLicensePlate,
        NotificationGeneralType generalType,
        NotificationVehicleType vehicleType = NotificationVehicleType.Other,
        DateTime? triggerDate = null,
        string? contactIdentifier = null,
        Dictionary<string, string> metadata = null!
    )
    {
        VehicleLicensePlate = vehicleLicensePlate;
        GeneralType = generalType;
        VehicleType = vehicleType;
        TriggerDate = triggerDate;
        ContactIdentifier = contactIdentifier;
        Metadata = metadata;
    }

    public bool IsRecurring { get; set; }

    public string VehicleLicensePlate { get; set; } = null!;

    public NotificationGeneralType GeneralType { get; set; }

    public NotificationVehicleType VehicleType { get; set; }

    public DateTime? TriggerDate { get; set; }

    public string? ContactIdentifier { get; set; } = null;

    public Dictionary<string, string> Metadata { get; set; } = null!;

    [JsonIgnore]
    public VehicleLookupItem? VehicleLookup { get; set; } = null!;

}

public class CreateNotificationMessageCommandHandler : IRequestHandler<CreateNotificationCommand, NotificationItem>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentificationHelper _identificationHelper;

    public CreateNotificationMessageCommandHandler(IApplicationDbContext context, IIdentificationHelper identificationHelper)
    {
        _context = context;
        _identificationHelper = identificationHelper;
    }

    public async Task<NotificationItem> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        var receiver = request.ContactIdentifier!;
        var receiverType = receiver.GetContactType();

        var notification = new NotificationItem
        {
            GeneralType = request.GeneralType,
            VehicleType = request.VehicleType,
            TriggerDate = request.TriggerDate,
            ReceiverContactType = receiverType,
            ReceiverContactIdentifier = receiver,
            VehicleLicensePlate = request.VehicleLicensePlate,
            Metadata = request.Metadata
        };

        // store all notifications, to be able to track them
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(cancellationToken);

        return notification;
    }

}
