using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Interfaces.Messaging;
using AutoHelper.Domain.Entities.Conversations;
using Microsoft.EntityFrameworkCore;
using WhatsappBusiness.CloudApi.Exceptions;
using WhatsappBusiness.CloudApi.Interfaces;
using WhatsappBusiness.CloudApi.Messages.ReplyRequests;
using WhatsappBusiness.CloudApi.Messages.Requests;
using TextMessageContext = WhatsappBusiness.CloudApi.Messages.ReplyRequests.TextMessageContext;

namespace AutoHelper.Messaging.Services;

/// <summary>
/// For more information about the webhook, please refer to the documentation: https://developers.facebook.com/docs/whatsapp/api/webhooks/inbound
/// And our github client: https://github.com/gabrieldwight/Whatsapp-Business-Cloud-Api-Net?tab=readme-ov-file
/// </summary>
internal class WhatsappResponseService : IWhatsappResponseService
{
    private readonly IApplicationDbContext _context;
    private readonly IWhatsAppBusinessClient _whatsAppBusinessClient;

    public WhatsappResponseService(IApplicationDbContext context, IWhatsAppBusinessClient whatsAppBusinessClient)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _whatsAppBusinessClient = whatsAppBusinessClient ?? throw new ArgumentNullException(nameof(whatsAppBusinessClient));
    }

    /// <summary>
    /// Update the message id of the conversation message.
    /// </summary>
    public async Task UpdateMessageId(ConversationMessageItem message, string messageId, CancellationToken cancellationToken, bool skipWhenExist = true)
    {
        if (skipWhenExist && !string.IsNullOrEmpty(message.WhatsappMessageId))
        {
            return;
        }

        message.WhatsappMessageId = messageId;
        _context.ConversationMessages.Update(message);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Get the referd conversation id from the message id.
    /// </summary>
    public async Task<Guid?> GetValidatedConversationId(string identifier, string messageId, string? contextMessageId = null)
    {
        if (string.IsNullOrEmpty(contextMessageId))
        {
            var message = "Kunt u aangeven naar welke garage u een bericht wilt sturen? Stuur alstublieft uw bericht opnieuw, met een verwijzing naar ons eerdere gesprek, zodat we kunnen begrijpen waar het over gaat.";
            await SendSimpleMessage(identifier, message, contextMessageId);

            return null;
        }

        var conversationId = await _context.ConversationMessages
            .AsNoTracking()
            .Where(x => x.WhatsappMessageId == contextMessageId)
            .Select(x => x.ConversationId)
            .FirstOrDefaultAsync();

        if (conversationId == default)
        {
            var message = "We kunnen helaas niet opmaken waar uw verwijzing naar verwijst. Bezoek alstublieft https://autohelper.nl om een nieuw gesprek te starten.";
            await SendSimpleMessage(identifier, message, contextMessageId);

            return null;
        }

        return conversationId;
    }

    /// <summary>
    /// Mark the message as read on whatsapp.
    /// </summary>
    public async Task MarkMessageAsRead(string messageId)
    {
        var request = new MarkMessageRequest
        {
            MessageId = messageId,
            Status = "read"
        };

        await _whatsAppBusinessClient.MarkMessageAsReadAsync(request);
    }

    /// <summary>
    /// Can been used to send a message to the receiver when the receiver did first send a message, otherwise use 
    /// the WhatsappTemplateService
    /// </summary>
    /// <param name="identifier">is the receiver identifier, looks like 31618395668</param>
    /// <param name="message">Mesage that will been send to the receiver</param>
    /// <param name="contextMessageId">when giving response on previous sended message</param>
    private async Task SendSimpleMessage(string identifier, string message, string? contextMessageId = null)
    {
        try
        {
            var request = new TextMessageReplyRequest
            {
                To = identifier,
                Text = new WhatsAppText
                {
                    Body = message,
                    PreviewUrl = false
                }
            };

            if (!string.IsNullOrEmpty(contextMessageId))
            {
                request.Context = new TextMessageContext
                {
                    MessageId = contextMessageId
                };
            }

            await _whatsAppBusinessClient.SendTextMessageAsync(request);
        }
        catch (WhatsappBusinessCloudAPIException)
        {
            throw;
        }
    }

}