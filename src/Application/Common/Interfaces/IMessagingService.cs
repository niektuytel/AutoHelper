﻿using AutoHelper.Application.Messages._DTOs;
using AutoHelper.Domain.Entities.Conversations;

namespace AutoHelper.Application.Common.Interfaces;

public interface IMessagingService
{
    Task SendMessage(ConversationMessageItem message, string senderName, CancellationToken cancellationToken);
    Task SendMessageWithVehicle(ConversationMessageItem message, VehicleTechnicalDtoItem vehicle, CancellationToken cancellationToken);
    Task SendMessageConfirmation(ConversationMessageItem message, string receiverName, CancellationToken cancellationToken);
}