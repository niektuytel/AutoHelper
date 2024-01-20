using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading;
using AutoHelper.Application.Common.Enums;
using AutoHelper.Application.Common.Extensions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Conversations.Commands.CreateConversationMessage;
using AutoHelper.Application.Conversations.Commands.CreateGarageConversationItems;
using AutoHelper.Application.Conversations.Commands.ReceiveMessage;
using AutoHelper.Application.Conversations.Commands.SendMessage;
using AutoHelper.Hangfire.MediatR;
using Azure.Core;
using Hangfire;
using HtmlAgilityPack;
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

public class ConversationController : ApiControllerBase
{
    const string VerifyToken = "Autohelper";
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IWhatsappResponseService _whatsappResponseService;
    private readonly ILogger<ConversationController> _logger;

    public ConversationController(IBackgroundJobClient backgroundJobClient, IWhatsappResponseService whatsappResponseService, ILogger<ConversationController> logger)
    {
        _backgroundJobClient = backgroundJobClient;
        _whatsappResponseService = whatsappResponseService;
        _logger = logger;
    }

    [HttpPost($"{nameof(ReceiveEmailMessage)}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    public async Task<string> ReceiveEmailMessage([FromBody] ReceiveEmailMessageCommand message, CancellationToken cancellationToken)
    {
        var conversationMessage = await Mediator.Send(message, cancellationToken);

        var queue = nameof(SendMessageCommand);
        var messageCommand = new SendMessageCommand(conversationMessage);
        Mediator.Enqueue(_backgroundJobClient, queue, messageCommand.Title, messageCommand);

        return $"Conversation-ID: {conversationMessage.ConversationId}";
    }

    [HttpGet(nameof(ReceiveWhatsappMessage))]
    public ActionResult<string> ReceiveWhatsappMessage(
        [FromQuery(Name = "hub.mode")] string hubMode,
        [FromQuery(Name = "hub.challenge")] int hubChallenge,
        [FromQuery(Name = "hub.verify_token")] string hubVerifyToken
    )
    {
        _logger.LogInformation("Results Returned from WhatsApp Server\n");
        _logger.LogInformation($"hub_mode={hubMode}\n");
        _logger.LogInformation($"hub_challenge={hubChallenge}\n");
        _logger.LogInformation($"hub_verify_token={hubVerifyToken}\n");

        if (!hubVerifyToken.Equals(VerifyToken))
        {
            return Forbid("VerifyToken doesn't match");
        }

        return Ok(hubChallenge);
    }

    /// <summary>
    /// We only accept text messages for now:
    /// For more information about the webhook, please refer to the documentation: https://developers.facebook.com/docs/whatsapp/api/webhooks/inbound
    /// And our github client: https://github.com/gabrieldwight/Whatsapp-Business-Cloud-Api-Net?tab=readme-ov-file
    /// </summary>
    [HttpPost(nameof(ReceiveWhatsappMessage))]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReceiveWhatsappMessage([FromBody] TextMessageReceived? body, CancellationToken cancellationToken) 
    {
        var messageType = body?.Entry?[0].Changes?[0].Value?.Messages?[0].Type;
        if (messageType != "text")
        {
            return BadRequest(new { Message = "We only accept text messages" });
        }

        var messages = body!.Entry
            .SelectMany(x => x.Changes)
            .SelectMany(x => x.Value.Messages);

        foreach (var message in messages)
        {
            await _whatsappResponseService.MarkMessageAsRead(message.Id);

            if(message.Context == null)
            {
                await _whatsappResponseService.SendErrorMessage(WhatsappMessageErrorType.ContextMessageIdNotFound);
                return Ok(new { Message = "This message does not have any related context" });
            }
            else
            {
                var conversationId = await _whatsappResponseService.GetConversationId(message.Context.Id);
                if (conversationId == null)
                {
                    await _whatsappResponseService.SendErrorMessage(WhatsappMessageErrorType.ConversationNotFound);
                    return Ok(new { Message = "This message does not contain a valid Referentie-Id" });
                }

                // get the conversation id from the context, can been nested in the above it
                var createMessage = new ReceiveWhatsappMessageCommand()
                {
                    ConversationId = (Guid)conversationId!,
                    WhatsappMessageId = message.Id,
                    From = message.From,
                    Body = message.Text.Body,
                };

                var conversationMessage = await Mediator.Send(createMessage, cancellationToken);

                var queue = nameof(SendMessageCommand);
                var messageCommand = new SendMessageCommand(conversationMessage);
                Mediator.Enqueue(_backgroundJobClient, queue, messageCommand.Title, messageCommand);
            }
        }


        return Ok(new { Message = "Handling message" });
    }

    [HttpPost($"{nameof(StartGarageConversation)}")]
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

            var queue = nameof(SendMessageCommand);
            var messageCommand = new SendMessageCommand(message.Id);
            Mediator.Enqueue(_backgroundJobClient, queue, messageCommand.Title, messageCommand);
        }

        return $"Conversation-IDs: [{string.Join(", ", conversationIds)}]";
    }

}
