using AutoHelper.Application.Messages._DTOs;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Messages;
using AutoHelper.WebUI.Controllers;

namespace AutoHelper.Application.Common.Interfaces;

public interface IMessagingService
{
    Task SendMessage(ConversationMessageItem message, string senderName, CancellationToken cancellationToken);
    Task SendMessageWithVehicle(ConversationMessageItem message, VehicleTechnicalDtoItem vehicle, CancellationToken cancellationToken);
    Task SendMessageConfirmation(ConversationMessageItem message, string receiverName, CancellationToken cancellationToken);
    Task SendNotificationMessage(NotificationItem notification, CancellationToken cancellationToken);
}