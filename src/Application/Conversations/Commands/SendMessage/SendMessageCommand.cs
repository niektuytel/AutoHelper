using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Conversations.Enums;
using MediatR;
using Microsoft.VisualBasic;
using System.Net.Mail;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading;

namespace AutoHelper.Application.Conversations.Commands.SendMessage;

public record SendMessageCommand : IRequest<string>
{
    public SendMessageCommand(Guid conversationMessageId)
    {
        ConversationMessageId = conversationMessageId;
    }

    public Guid ConversationMessageId { get; private set; }

    [JsonIgnore]
    internal ConversationMessageItem? ConversationMessage { get; set; }

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
        var senderService = GetMessagingService(request, fromSender: true);
        var receiverService = GetMessagingService(request, fromSender: false);

        var sendToGarage = DetermineRecipientIsGarage(request);
        var senderContactName = GetSenderContactName(request.ConversationMessage!.Conversation, sendToGarage);
        if (sendToGarage)
        {
            var licensePlate = request.ConversationMessage!.Conversation.RelatedVehicleLookup.LicensePlate;
            var vehicleInfo = await _vehicleService.GetTechnicalBriefByLicensePlateAsync(licensePlate);
            if (vehicleInfo == null)
            {
                throw new InvalidDataException($"Vehicle not found: {licensePlate}");
            }

            await senderService.SendMessageWithVehicle(
                request.ConversationMessage.ReceiverContactIdentifier,
                request.ConversationMessage.ConversationId,
                vehicleInfo,
                request.ConversationMessage.MessageContent
            );
        }
        else
        {
            await senderService.SendMessage(
                request.ConversationMessage!.ReceiverContactIdentifier,
                request.ConversationMessage.ConversationId,
                senderContactName,
                request.ConversationMessage.MessageContent
            );
        }

        await receiverService.SendMessageConfirmation(
            request.ConversationMessage.SenderContactIdentifier, 
            request.ConversationMessage.ConversationId, 
            senderContactName
        );

        await UpdateDatabaseMessage(request, cancellationToken);
        return $"Message sended to: {request.ConversationMessage!.ReceiverContactIdentifier}";
    }

    private async Task UpdateDatabaseMessage(SendMessageCommand request, CancellationToken cancellationToken)
    {
        request.ConversationMessage!.Status = MessageStatus.Delivered;

        _context.ConversationMessages.Update(request.ConversationMessage);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static bool DetermineRecipientIsGarage(SendMessageCommand request)
    {
        return request.ConversationMessage!.ReceiverContactIdentifier == request.ConversationMessage.Conversation!.RelatedGarage.ConversationContactEmail ||
               request.ConversationMessage.ReceiverContactIdentifier == request.ConversationMessage.Conversation.RelatedGarage.ConversationContactWhatsappNumber;
    }

    private static string GetSenderContactName(ConversationItem conversation, bool sendingMessageToGarage)
    {
        if (sendingMessageToGarage)
        {
            return conversation.RelatedVehicleLookup.LicensePlate;
        }

        return conversation.RelatedGarage.Name;
    }
    
    private IMessagingService GetMessagingService(SendMessageCommand request, bool fromSender)
    {
        var contactType = fromSender ? 
            request.ConversationMessage!.SenderContactType 
            : 
            request.ConversationMessage!.ReceiverContactType;

        return contactType switch
        {
            ContactType.Email => _mailingService,
            ContactType.WhatsApp => _whatsappService,
            _ => throw new InvalidOperationException($"Invalid contact type: {request.ConversationMessage!.ReceiverContactType}"),
        };
    }
}
