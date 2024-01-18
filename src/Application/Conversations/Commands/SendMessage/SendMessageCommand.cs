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

namespace AutoHelper.Application.Conversations.Commands.SendMessage;

public record SendMessageCommand : IQueueRequest<string>
{
    public SendMessageCommand()
    {
        
    }

    public SendMessageCommand(Guid messageId)
    {
        MessageId = messageId;
    }

    public SendMessageCommand(ConversationMessageItem message)
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

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly IWhatsappService _whatsappService;
    private readonly IMailingService _mailingService;
    private readonly IVehicleService _vehicleService;

    public SendMessageCommandHandler(
        IApplicationDbContext context,
        IWhatsappService whatsappService,
        IMailingService mailingService,
        IVehicleService vehicleService
    )
    {
        _context = context;
        _whatsappService = whatsappService;
        _mailingService = mailingService;
        _vehicleService = vehicleService;
    }

    public async Task<string> Handle(SendMessageCommand request, CancellationToken cancellationToken)
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
                sendToGarage
            );
        }
        else
        {
            //// M > M [OK]
            //await (senderService as IMailingService)!.SendMessageRaw(
            //    request.Message!.ReceiverContactIdentifier,
            //    request.Message.ConversationId,
            //    senderContactName,
            //    request.Message.MessageContent
            //);

            // M > W []
            // W > M []
            // W > W []
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
        bool sendToGarage
    ) {
        if (sendToGarage)
        {
            var licensePlate = message!.Conversation.VehicleLicensePlate;
            var vehicleInfo = await _vehicleService.GetTechnicalBriefByLicensePlateAsync(licensePlate);
            if (vehicleInfo == null)
            {
                throw new InvalidDataException($"Vehicle not found: {licensePlate}");
            }

            await senderService.SendMessageWithVehicle(
                message.ReceiverContactIdentifier,
                message.ConversationId,
                vehicleInfo,
                message.MessageContent
            );
        }
        else
        {
            await senderService.SendMessage(
                message!.ReceiverContactIdentifier,
                message.ConversationId,
                senderContactName,
                message.MessageContent
            );
        }

        await receiverService.SendMessageConfirmation(
            message!.SenderContactIdentifier,
            message.ConversationId,
            receiverContactName
        );
    }

    private async Task UpdateDatabaseMessage(SendMessageCommand request, CancellationToken cancellationToken)
    {
        request.Message!.Status = MessageStatus.Delivered;

        _context.ConversationMessages.Update(request.Message);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static bool DetermineRecipientIsGarage(SendMessageCommand request)
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
    
    private IMessagingService GetMessagingService(SendMessageCommand request, bool fromSender)
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
