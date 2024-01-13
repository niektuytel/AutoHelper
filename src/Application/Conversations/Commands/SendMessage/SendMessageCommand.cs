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
        var sendToGarage = DetermineRecipientIsGarage(request);
        var senderContactName = GetSenderContactName(request.ConversationMessage!.Conversation, sendToGarage);

        await SendMessage(request, sendToGarage, senderContactName);
        await SendMessageReceiptToSender(request, senderContactName);

        await UpdateDatabaseMessage(request, cancellationToken);

        return $"Message sended to: {request.ConversationMessage!.ReceiverContactIdentifier}";
    }

    private async Task UpdateDatabaseMessage(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var message = new ConversationMessageItem
        {
            ConversationId = request.ConversationMessage!.Conversation!.Id,
            SenderContactType = request.ConversationMessage!.SenderContactType,
            SenderContactIdentifier = request.ConversationMessage!.SenderContactIdentifier,
            ReceiverContactType = request.ConversationMessage!.ReceiverContactType,
            ReceiverContactIdentifier = request.ConversationMessage!.ReceiverContactIdentifier,
            Status = MessageStatus.Sent,
            MessageContent = request.ConversationMessage!.MessageContent,
        };

        _context.ConversationMessages.Add(message);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task SendMessage(SendMessageCommand request, bool sendToGarage, string senderContactName)
    {
        switch (request.ConversationMessage!.ReceiverContactType)
        {
            case ContactType.Email:
                await SendEmail(request, sendToGarage, senderContactName);
                break;
            case ContactType.WhatsApp:
                await SendWhatsappMessage(request, sendToGarage, senderContactName);
                break;
            default:
                throw new InvalidOperationException($"Invalid contact type: {request.ConversationMessage!.ReceiverContactType}");
        }
    }

    private async Task SendMessageReceiptToSender(SendMessageCommand request, string senderContactName)
    {
        switch (request.ConversationMessage!.SenderContactType)
        {
            case ContactType.Email:
                await _mailingService.SendConfirmationEmailAsync(request.ConversationMessage.SenderContactIdentifier, request.ConversationMessage.ConversationId, senderContactName);
                break;
            case ContactType.WhatsApp:
                await _whatsappService.SendConfirmationMessageAsync(request.ConversationMessage.SenderContactIdentifier, request.ConversationMessage.ConversationId, senderContactName);
                break;
            default:
                throw new InvalidOperationException($"Invalid contact type: {request.ConversationMessage.ReceiverContactType}");
        }
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
    
    // TODO: Refactor this to a service into the MessageService
    private async Task SendEmail(SendMessageCommand request, bool sendToGarage, string senderContactName)
    {
        // https://developer.microsoft.com/en-us/graph/graph-explorer

        var client = new SmtpClient("smtp.autohelper.nl", 587)
        {
            Credentials = new NetworkCredential("contact@autohelper.com", "Auto1337!"),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress("contact@autohelper.com"),
            Subject = "Subject Here",
            Body = "Your Email Template Here",
            IsBodyHtml = true
        };
        mailMessage.To.Add("ntuijtel@gmail.com");

        client.Send(mailMessage);



        //if (sendToGarage)
        //{
        //    var licensePlate = conversation.RelatedVehicleLookup.LicensePlate;
        //    var vehicleInfo = await _vehicleService.GetTechnicalBriefByLicensePlateAsync(licensePlate);
        //    if (vehicleInfo == null)
        //    {
        //        throw new InvalidDataException($"Vehicle not found: {licensePlate}");
        //    }

        //    await _mailingService.SendVehicleRelatedEmailAsync(
        //        request.ReceiverContactIdentifier,
        //        request.ConversationId,
        //        vehicleInfo,
        //        request.MessageContent
        //    );
        //}
        //else
        //{
        //    await _mailingService.SendBasicMailAsync(
        //        request.ReceiverContactIdentifier,
        //        request.ConversationId,
        //        senderContactName,
        //        request.MessageContent
        //    );
        //}
    }

    // TODO: Refactor this to a service into the MessageService
    private async Task SendWhatsappMessage(SendMessageCommand request, bool sendToGarage, string senderContactName)
    {
        if (sendToGarage)
        {
            var licensePlate = request.ConversationMessage!.Conversation.RelatedVehicleLookup.LicensePlate;
            var vehicleInfo = await _vehicleService.GetTechnicalBriefByLicensePlateAsync(licensePlate);
            if (vehicleInfo == null)
            {
                throw new InvalidDataException($"Vehicle not found: {licensePlate}");
            }

            await _whatsappService.SendVehicleRelatedMessageAsync(
                request.ConversationMessage!.ReceiverContactIdentifier,
                request.ConversationMessage!.ConversationId,
                vehicleInfo,
                request.ConversationMessage!.MessageContent
            );
        }
        else
        {
            await _whatsappService.SendBasicMessageAsync(
                request.ConversationMessage!.ReceiverContactIdentifier,
                request.ConversationMessage!.ConversationId,
                senderContactName,
                request.ConversationMessage!.MessageContent
            );
        }
    }

}
