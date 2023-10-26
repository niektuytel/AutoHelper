using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Application.Messages.Commands.SendConfirmationMessage;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Conversations.Enums;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace AutoHelper.Application.Messages.Commands.SendConversationMessage;

public record SendConversationMessageCommand : IRequest<SendConfirmationMessageCommand?>
{
    public SendConversationMessageCommand(
        Guid conversationId,
        ContactType senderContactType,
        string senderContactIdentifier,
        ContactType receiverContactType,
        string receiverContactIdentifier,
        string messageContent,
        int messageIndex = 1
    )
    {
        ConversationId = conversationId;
        SenderContactType = senderContactType;
        SenderContactIdentifier = senderContactIdentifier;
        ReceiverContactType = receiverContactType;
        ReceiverContactIdentifier = receiverContactIdentifier;
        MessageIndex = messageIndex;
        Timestamp = DateTime.UtcNow;
        Status = MessageStatus.Pending;
        MessageContent = messageContent;
    }

    public SendConversationMessageCommand(
        Guid conversationId,
        ContactType senderContactType,
        string senderContactIdentifier,
        ContactType receiverContactType,
        string receiverContactIdentifier,
        int messageIndex,
        DateTime dateTime,
        MessageStatus status,
        string messageContent,
        string? errorMessage
    )
    {
        ConversationId = conversationId;
        SenderContactType = senderContactType;
        SenderContactIdentifier = senderContactIdentifier;
        ReceiverContactType = receiverContactType;
        ReceiverContactIdentifier = receiverContactIdentifier;
        MessageIndex = messageIndex;
        Timestamp = dateTime;
        Status = status;
        MessageContent = messageContent;
        ErrorMessage = errorMessage;
    }

    public Guid ConversationId { get; private set; }

    public ContactType SenderContactType { get; private set; }
    public string SenderContactIdentifier { get; private set; }

    public ContactType ReceiverContactType { get; private set; }
    public string ReceiverContactIdentifier { get; private set; }

    public int MessageIndex { get; private set; }
    public DateTime Timestamp { get; private set; }
    public MessageStatus Status { get; private set; }

    public string MessageContent { get; private set; }
    public string? ErrorMessage { get; private set; }

}

public class StartConversationCommandHandler : IRequestHandler<SendConversationMessageCommand, SendConfirmationMessageCommand?>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IWhatsappService _whatsappService;
    private readonly IMailingService _mailingService;
    private readonly IVehicleService _vehicleService;

    public StartConversationCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IWhatsappService whatsappService,
        IMailingService mailingService,
        IVehicleService vehicleService
    )
    {
        _context = context;
        _mapper = mapper;
        _whatsappService = whatsappService;
        _mailingService = mailingService;
        _vehicleService = vehicleService;
    }

    public async Task<SendConfirmationMessageCommand?> Handle(SendConversationMessageCommand request, CancellationToken cancellationToken)
    {
        var conversation = _context.Conversations
            .Include(x => x.RelatedGarageLookup)
            .Include(x => x.RelatedVehicleLookup)
            .FirstOrDefault(x => x.Id == request.ConversationId);

        if (conversation == null)
        {
            throw new InvalidDataException("Conversation not found");
        }

        var sendToGarage = HasMatchingContact(request.ReceiverContactIdentifier, conversation.RelatedGarageLookup);
        var senderContactName = GetSenderContactName(conversation, sendToGarage);

        await SendMessage(request, conversation, sendToGarage, senderContactName);

        return new SendConfirmationMessageCommand(
            request.ConversationId,
            senderContactName,
            request.SenderContactType,
            request.SenderContactIdentifier,
            request.MessageContent
        );
    }

    private async Task SendMessage(SendConversationMessageCommand request, ConversationItem conversation, bool sendToGarage, string senderContactName)
    {
        switch (request.ReceiverContactType)
        {
            case ContactType.Email:
                await SendEmail(request, conversation, sendToGarage, senderContactName);
                break;
            case ContactType.WhatsApp:
                await SendWhatsAppMessage(request, conversation, sendToGarage, senderContactName);
                break;
            default:
                throw new InvalidOperationException($"Invalid contact type: {request.ReceiverContactType}");
        }
    }

    private async Task SendEmail(SendConversationMessageCommand request, ConversationItem conversation, bool sendToGarage, string senderContactName)
    {
        var subject = $"AutoHelper - {conversation.RelatedGarageLookup.Name} - {conversation.RelatedVehicleLookup.LicensePlate}";
        if (sendToGarage)
        {
            var licensePlate = conversation.RelatedVehicleLookup.LicensePlate;
            var vehicleInfo = await _vehicleService.GetVehicleTechnicalBriefInfo(licensePlate);
            if (vehicleInfo == null)
            {
                throw new InvalidDataException($"Vehicle not found: {licensePlate}");
            }

            await _mailingService.SendVehicleRelatedEmailAsync(
                request.ReceiverContactIdentifier,
                request.ConversationId,
                vehicleInfo,
                subject,
                request.MessageContent
            );
        }
        else
        {
            await _mailingService.SendBasicMailAsync(
                request.ReceiverContactIdentifier,
                request.ConversationId,
                senderContactName,
                subject,
                request.MessageContent
            );
        }
    }

    private async Task SendWhatsAppMessage(SendConversationMessageCommand request, ConversationItem conversation, bool sendToGarage, string senderContactName)
    {
        if (sendToGarage)
        {
            var licensePlate = conversation.RelatedVehicleLookup.LicensePlate;
            var vehicleInfo = await _vehicleService.GetVehicleTechnicalBriefInfo(licensePlate);
            if (vehicleInfo == null)
            {
                throw new InvalidDataException($"Vehicle not found: {licensePlate}");
            }

            await _whatsappService.SendVehicleRelatedMessageAsync(
                request.ReceiverContactIdentifier,
                request.ConversationId,
                vehicleInfo,
                request.MessageContent
            );
        }
        else
        {
            await _whatsappService.SendBasicMessageAsync(
                request.ReceiverContactIdentifier,
                request.ConversationId,
                senderContactName,
                request.MessageContent
            );
        }
    }

    private static bool HasMatchingContact(string identifier, GarageLookupItem garageLookup)
    {
        if (garageLookup.PhoneNumber == identifier)
        {
            return true;
        }

        if (garageLookup.WhatsappNumber == identifier)
        {
            return true;
        }

        if (garageLookup.EmailAddress == identifier)
        {
            return true;
        }

        return false;
    }

    private static string GetSenderContactName(ConversationItem conversation, bool sendingMessageToGarage)
    {
        if (sendingMessageToGarage)
        {
            return conversation.RelatedVehicleLookup.LicensePlate;
        }

        return conversation.RelatedGarageLookup.Name;
    }
}
