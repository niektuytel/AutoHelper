using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Conversations.Enums;
using MediatR;
using Microsoft.VisualBasic;
using System.Net.Mail;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using static System.Net.Mime.MediaTypeNames;

namespace AutoHelper.Application.Conversations.Commands.SendMessage;

public record SendConversationMessageCommand : IQueueRequest<string>
{
    public SendConversationMessageCommand()
    {
        
    }

    public SendConversationMessageCommand(Guid messageId)
    {
        MessageId = messageId;
    }

    public SendConversationMessageCommand(ConversationMessageItem message)
    {
        MessageId = message.Id;
        Message = message;
    }

    public Guid MessageId { get; set; }

    [JsonIgnore]
    internal ConversationMessageItem? Message { get; set; } = null;

    [JsonIgnore]
    public string Title => $"[{Message?.SenderContactIdentifier}] To [{Message?.ReceiverContactIdentifier}]";

    [JsonIgnore]
    public IQueueService QueueingService { get; set; } = null!;
}

public class SendMessageCommandHandler : IRequestHandler<SendConversationMessageCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly IWhatsappTemplateService _whatsappService;
    private readonly IMailingService _mailingService;
    private readonly IVehicleService _vehicleService;

    public SendMessageCommandHandler(
        IApplicationDbContext context,
        IWhatsappTemplateService whatsappService,
        IMailingService mailingService,
        IVehicleService vehicleService
    )
    {
        _context = context;
        _whatsappService = whatsappService;
        _mailingService = mailingService;
        _vehicleService = vehicleService;
    }

    public async Task<string> Handle(SendConversationMessageCommand request, CancellationToken cancellationToken)
    {
        var sendToGarage = DetermineRecipientIsGarage(request);
        var senderService = GetMessagingService(request, fromSender: true);
        var receiverService = GetMessagingService(request, fromSender: false);
        var senderContactName = GetSenderContactName(request.Message!.Conversation, sendToGarage);
        var receiverContactName = GetSenderContactName(request.Message!.Conversation, !sendToGarage);

        var totalMessagesInConversation = await _context.ConversationMessages.CountAsync(x =>
            x.ConversationId == request.Message!.ConversationId,
            cancellationToken
        );

        var isFirstMessage = totalMessagesInConversation == 1;
        if (isFirstMessage)
        { 
            await HandleFirstMessage(
                senderService,
                receiverService,
                request.Message,
                senderContactName,
                receiverContactName,
                sendToGarage,
                cancellationToken
            );
        }
        else
        {
            await receiverService.SendMessage(
                request.Message, 
                senderContactName, 
                cancellationToken
            );
        }

        await UpdateDatabaseMessage(request, cancellationToken);

        return $"Message sended to: {request.Message!.ReceiverContactIdentifier}";
    }

    private async Task HandleFirstMessage(
        IMessagingService senderService, 
        IMessagingService receiverService, 
        ConversationMessageItem message,
        string senderContactName,
        string receiverContactName,
        bool sendToGarage,
        CancellationToken cancellationToken
    ) {
        if (sendToGarage)
        {
            var licensePlate = message!.Conversation.VehicleLicensePlate;
            var vehicle = await _vehicleService.GetTechnicalBriefByLicensePlateAsync(licensePlate);
            if (vehicle == null)
            {
                throw new InvalidDataException($"Vehicle not found: {licensePlate}");
            }

            await receiverService.SendMessageWithVehicle(message, vehicle, cancellationToken);
        }
        else
        {
            await receiverService.SendMessage(message, senderContactName, cancellationToken);
        }

        await senderService.SendMessageConfirmation(message, receiverContactName, cancellationToken);
    }

    private async Task UpdateDatabaseMessage(SendConversationMessageCommand request, CancellationToken cancellationToken)
    {
        request.Message!.Status = MessageStatus.Delivered;

        _context.ConversationMessages.Update(request.Message);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static bool DetermineRecipientIsGarage(SendConversationMessageCommand request)
    {
        return request.Message!.ReceiverContactIdentifier == request.Message.Conversation!.RelatedGarage.ConversationContactEmail ||
               request.Message.ReceiverContactIdentifier == request.Message.Conversation.RelatedGarage.ConversationContactWhatsappNumber;
    }

    private static string GetSenderContactName(ConversationItem conversation, bool sendingMessageToGarage)
    {
        if (sendingMessageToGarage)
        {
            return conversation.VehicleLicensePlate;
        }

        return conversation.RelatedGarage.Name;
    }
    
    private IMessagingService GetMessagingService(SendConversationMessageCommand request, bool fromSender)
    {
        var contactType = fromSender ? 
            request.Message!.SenderContactType 
            : 
            request.Message!.ReceiverContactType;

        return contactType switch
        {
            ContactType.Email => _mailingService,
            ContactType.WhatsApp => _whatsappService,
            _ => throw new InvalidOperationException($"Invalid contact type: {request.Message!.ReceiverContactType}"),
        };
    }
}
