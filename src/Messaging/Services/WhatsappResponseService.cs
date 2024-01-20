using System.Net.Http;
using System.Text.RegularExpressions;
using AutoHelper.Application.Common.Enums;
using AutoHelper.Application.Common.Extensions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Conversations._DTOs;
using AutoHelper.Domain.Entities.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WhatsappBusiness.CloudApi;
using WhatsappBusiness.CloudApi.Exceptions;
using WhatsappBusiness.CloudApi.Interfaces;
using WhatsappBusiness.CloudApi.Messages.ReplyRequests;
using WhatsappBusiness.CloudApi.Messages.Requests;
using WhatsappBusiness.CloudApi.Webhook;

namespace AutoHelper.Messaging.Services;

internal class WhatsappResponseService : IWhatsappResponseService
{
    private readonly IApplicationDbContext _context;
    private readonly IWhatsAppBusinessClient _whatsAppBusinessClient;
    private readonly IConfiguration _configuration;
    private readonly bool _isDevelopment;
    private readonly string _developPhoneNumberId;

    public WhatsappResponseService(IApplicationDbContext context, IWhatsAppBusinessClient whatsAppBusinessClient, IConfiguration configuration)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _whatsAppBusinessClient = whatsAppBusinessClient ?? throw new ArgumentNullException(nameof(whatsAppBusinessClient));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task SetMessageIdWhenEmpty(Guid conversationMessageId, string messageId, CancellationToken cancellationToken)
    {
        var entity = _context.ConversationMessages
            .AsNoTracking()
            .Where(x => x.Id == conversationMessageId)
            .FirstOrDefault();

        if (entity == null)
        {
            throw new Exception($"ConversationMessage with id {conversationMessageId} not found");
        }

        if (string.IsNullOrEmpty(entity.WhatsappMessageId))
        {
            entity.WhatsappMessageId = messageId;
            _context.ConversationMessages.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<Guid?> GetConversationId(string contextMessageId)
    {
        var conversationId = await _context.ConversationMessages
            .AsNoTracking()
            .Where(x => x.WhatsappMessageId == contextMessageId)
            .Select(x => x.ConversationId)
            .FirstOrDefaultAsync();

        return conversationId == default ? null : conversationId;
    }

    public async Task MarkMessageAsRead(string messageId)
    {
        var markMessageRequest = new MarkMessageRequest
        {
            MessageId = messageId,
            Status = "read"
        };

        await _whatsAppBusinessClient.MarkMessageAsReadAsync(markMessageRequest);
    }

    public async Task SendErrorMessage(WhatsappMessageErrorType type)
    {
        // ErrorMessageType.ContextMessageIdNotFound
        // ERROR: ask if the user can reply on the message to know which garage to talk to

        // ErrorMessageType.ConversationNotFound);
        // ERROR: this message does not contain a valid Referentie-Id

        //var markMessageRequest = new MarkMessageRequest
        //{
        //    MessageId = messageId,
        //    Status = "read"
        //};

        //await _whatsAppBusinessClient.MarkMessageAsReadAsync(markMessageRequest);
    }



    //    try
    //    {

    //        TextMessageReplyRequest textMessageReplyRequest = new TextMessageReplyRequest();
    //textMessageReplyRequest.Context = new WhatsappBusiness.CloudApi.Messages.ReplyRequests.TextMessageContext();
    //        textMessageReplyRequest.Context.MessageId = messages.SingleOrDefault().Id;
    //        textMessageReplyRequest.To = messages.SingleOrDefault().From;
    //        textMessageReplyRequest.Text = new WhatsAppText();
    //textMessageReplyRequest.Text.Body = "Your Message was received. Processing the request shortly";
    //        textMessageReplyRequest.Text.PreviewUrl = false;

    //        await _whatsAppBusinessClient.SendTextMessageAsync(textMessageReplyRequest);
    //_logger.LogError(JsonConvert.SerializeObject(messages));

    //        return Ok(new
    //        {
    //            Message = "Text Message received"
    //        });
    //    }
    //    catch (WhatsappBusinessCloudAPIException ex)
    //    {
    //        return StatusCode((int)HttpStatusCode.InternalServerError, ex);
    //    }
}