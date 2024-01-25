using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading;
using AutoHelper.Application.Common.Extensions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Messages.Commands.CreateConversationMessage;
using AutoHelper.Application.Messages.Commands.CreateGarageConversationItems;
using AutoHelper.Application.Messages.Commands.CreateNotificationMessage;
using AutoHelper.Application.Messages.Commands.ReceiveMessage;
using AutoHelper.Application.Messages.Commands.SendConversationMessage;
using AutoHelper.Application.Vehicles.Commands.CreateVehicleEventNotifier;
using AutoHelper.Domain.Entities.Messages;
using AutoHelper.Domain.Entities.Messages.Enums;
using Azure.Core;
using GoogleApi.Entities.Interfaces;
using AutoHelper.Hangfire.Shared.MediatR;
using Hangfire;
using HtmlAgilityPack;
using IdentityModel;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WebUI.Models;
using WhatsappBusiness.CloudApi.Configurations;
using WhatsappBusiness.CloudApi.Exceptions;
using WhatsappBusiness.CloudApi.Interfaces;
using WhatsappBusiness.CloudApi.Messages.ReplyRequests;
using WhatsappBusiness.CloudApi.Messages.Requests;
using WhatsappBusiness.CloudApi.Webhook;

namespace AutoHelper.WebUI.Controllers;

public class MessageController : ApiControllerBase
{
    const string VerifyToken = "Autohelper";
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IWhatsappResponseService _whatsappResponseService;
    private readonly ILogger<MessageController> _logger;

    public MessageController(IBackgroundJobClient backgroundJobClient, IWhatsappResponseService whatsappResponseService, ILogger<MessageController> logger)
    {
        _backgroundJobClient = backgroundJobClient;
        _whatsappResponseService = whatsappResponseService;
        _logger = logger;
    }

    [HttpPost(nameof(ReceiveEmailMessage))]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    public async Task<string> ReceiveEmailMessage([FromBody] ReceiveEmailMessageCommand message, CancellationToken cancellationToken)
    {
        var conversationMessage = await Mediator.Send(message, cancellationToken);

        var queue = nameof(SendConversationMessageCommand);
        var messageCommand = new SendConversationMessageCommand(conversationMessage);
        Mediator.Enqueue(_backgroundJobClient, queue, messageCommand.Title, messageCommand);

        return $"Conversation-ID: {conversationMessage.ConversationId}";
    }

    [HttpGet(nameof(ReceiveWhatsappMessage))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public ActionResult<string> ReceiveWhatsappMessage(
        [FromQuery(Name = "hub.challenge")] int hubChallenge,
        [FromQuery(Name = "hub.verify_token")] string hubVerifyToken
    ) 
    {
        if (!hubVerifyToken.Equals(VerifyToken))
        {
            return Forbid("VerifyToken doesn't match");
        }

        return Ok(hubChallenge);
    }

    [HttpPost(nameof(ReceiveWhatsappMessage))]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReceiveWhatsappMessage(CancellationToken cancellationToken)
    {
        string requestBody;
        using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
        {
            requestBody = await reader.ReadToEndAsync(cancellationToken);
        }

        var body = JsonConvert.DeserializeObject<TextMessageReceived>(requestBody);
        if (body == null)
        {
            return Ok(new { Message = "We only accept text messages" });
        }

        var messageType = body?.Entry?[0].Changes?[0].Value?.Messages?[0].Type;
        if (messageType != "text")
        {
            return Ok(new { Message = "We only accept text messages with an type text" });
        }

        var messages = body!.Entry
            .SelectMany(x => x.Changes)
            .SelectMany(x => x.Value.Messages);

        foreach (var message in messages)
        {
            await _whatsappResponseService.MarkMessageAsRead(message.Id);

            var conversationId = await _whatsappResponseService.GetValidatedConversationId(message.From, message.Id, message?.Context?.Id);
            if (conversationId == null)
            {
                return Ok(new { Message = "This message does not contain a valid Referentie-Id" });
            }

            // get the conversation id from the context, can been nested in the above it
            var createMessage = new ReceiveWhatsappMessageCommand()
            {
                ConversationId = (Guid)conversationId!,
                WhatsappMessageId = message!.Id,
                From = message.From,
                Body = message.Text.Body,
            };

            try
            {
                var conversationMessage = await Mediator.Send(createMessage, cancellationToken);

                var queue = nameof(SendConversationMessageCommand);
                var messageCommand = new SendConversationMessageCommand(conversationMessage);
                Mediator.Enqueue(_backgroundJobClient, queue, messageCommand.Title, messageCommand);
            }
            catch (Exception ex)
            {
                return Ok(new { Message = ex.Message });
            }

        }


        return Ok(new { Message = "Handling message" });
    }

    [HttpPost(nameof(StartGarageConversation))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<string> StartGarageConversation([FromBody] CreateGarageConversationItemsCommand command, CancellationToken cancellationToken)
    {
        var conversations = await Mediator.Send(command, cancellationToken);
        var conversationIds = conversations.Select(x => x.Id).ToList();

        var sender = command.UserEmailAddress;
        if(string.IsNullOrWhiteSpace(command.UserEmailAddress))
        {
            sender = command.UserWhatsappNumber;
        }

        foreach (var conversation in conversations)
        {
            var message = conversation.Messages
                .OrderBy(item => item.LastModified)
                .LastOrDefault();

            if (message == null)
            {
                continue;
            }

            var queue = nameof(SendConversationMessageCommand);
            var messageCommand = new SendConversationMessageCommand(message.Id);
            Mediator.Enqueue(_backgroundJobClient, queue, messageCommand.Title, messageCommand);
        }

        return $"Conversation-IDs: [{string.Join(", ", conversationIds)}]";
    }

}
