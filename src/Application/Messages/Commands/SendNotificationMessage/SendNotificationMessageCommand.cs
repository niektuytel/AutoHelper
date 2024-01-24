using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Conversations.Enums;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;
using AutoHelper.Application.Messages._DTOs;
using Hangfire;
using AutoHelper.Application.Common.Extensions;
using AutoHelper.Application.Messages.Commands.SendConversationMessage;
using System.Text.Json.Serialization;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Messages;
using AutoHelper.Domain.Entities.Messages.Enums;
using AutoHelper.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AutoHelper.WebUI.Controllers;

namespace AutoHelper.Application.Messages.Commands.SendNotificationMessage;

public record SendNotificationMessageCommand : IRequest
{
    public SendNotificationMessageCommand(NotificationItemDto notification)
    {
        Notification = notification;
    }

    public NotificationItemDto Notification { get; set; }

}

public class SendNotificationMessageCommandHandler : IRequestHandler<SendNotificationMessageCommand>
{
    private readonly IWhatsappTemplateService _whatsappService;
    private readonly IMailingService _mailingService;

    public SendNotificationMessageCommandHandler(IWhatsappTemplateService whatsappService, IMailingService mailingService)
    {
        _whatsappService = whatsappService;
        _mailingService = mailingService;
    }

    public async Task<Unit> Handle(SendNotificationMessageCommand request, CancellationToken cancellationToken)
    {
        var receiverType = request.Notification.ReceiverContactType;
        var receiverService = GetMessagingService(receiverType);

        await receiverService.SendNotificationMessage(request.Notification, cancellationToken);

        return Unit.Value;
    }

    private IMessagingService GetMessagingService(ContactType contactType)
    {
        return contactType switch
        {
            ContactType.Email => _mailingService,
            ContactType.WhatsApp => _whatsappService,
            _ => throw new InvalidOperationException($"Invalid contact type: {contactType}"),
        };
    }

}
