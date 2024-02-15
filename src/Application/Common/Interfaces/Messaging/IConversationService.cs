using AutoHelper.Application.Messages._DTOs;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Messages;

namespace AutoHelper.Application.Common.Interfaces.Conversation;

public interface IConversationService
{
    Task SendMessage(ConversationMessageItem message, string senderName, CancellationToken cancellationToken);
    Task SendMessageWithVehicle(ConversationMessageItem message, VehicleTechnicalDtoItem vehicle, CancellationToken cancellationToken);
    Task SendMessageConfirmation(ConversationMessageItem message, string receiverName, CancellationToken cancellationToken);
}