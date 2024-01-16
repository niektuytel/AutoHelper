using AutoHelper.Application.Conversations._DTOs;

namespace AutoHelper.Application.Common.Interfaces;

public interface IMessagingService
{
    Task SendMessage(string receiverIdentifier, Guid conversationId, string senderName, string message);
    Task SendMessageWithVehicle(string receiverIdentifier, Guid conversationId, VehicleTechnicalDtoItem vehicle, string message);
    Task SendMessageConfirmation(string receiverIdentifier, Guid conversationId, string senderName);
}