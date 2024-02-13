using System.Text;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Messages.Commands.CreateGarageConversationItems;
using AutoHelper.Application.Messages.Commands.DeleteNotification;
using AutoHelper.Application.Messages.Commands.ReceiveMessage;
using AutoHelper.Application.Messages.Commands.SendConversationMessage;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WhatsappBusiness.CloudApi.Webhook;

namespace AutoHelper.WebUI.Controllers;

public class CommunicationController : ApiControllerBase
{
    const string VerifyToken = "Autohelper";
    private readonly IQueueService _queueService;
    private readonly IWhatsappResponseService _whatsappResponseService;
    private readonly ILogger<CommunicationController> _logger;

    public CommunicationController(IQueueService queueService, IWhatsappResponseService whatsappResponseService, ILogger<CommunicationController> logger)
    {
        _queueService = queueService;
        _whatsappResponseService = whatsappResponseService;
        _logger = logger;
    }

    [HttpPost(nameof(ReceiveEmailMessage))]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    public async Task<string> ReceiveEmailMessage([FromBody] ReceiveEmailMessageCommand message, CancellationToken cancellationToken)
    {
        var conversationMessage = await Mediator.Send(message, cancellationToken);

        var queue = nameof(SendConversationMessageCommand);
        var messageCommand = new SendConversationMessageCommand(conversationMessage);
        _queueService.Enqueue(queue, messageCommand.Title, messageCommand);

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
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
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
                _queueService.Enqueue(queue, messageCommand.Title, messageCommand);
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
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<string> StartGarageConversation([FromBody] CreateGarageConversationItemsCommand command, CancellationToken cancellationToken)
    {
        var conversations = await Mediator.Send(command, cancellationToken);
        var conversationIds = conversations.Select(x => x.Id).ToList();

        var sender = command.UserEmailAddress;
        if (string.IsNullOrWhiteSpace(command.UserEmailAddress))
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
            _queueService.Enqueue(queue, messageCommand.Title, messageCommand);
        }

        return $"Conversation-IDs: [{string.Join(", ", conversationIds)}]";
    }

    [HttpDelete($"{nameof(DeleteNotification)}/{{id}}")]
    [ProducesResponseType(typeof(NotificationItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<NotificationItemDto> DeleteNotification([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteNotificationCommand(id);
        return await Mediator.Send(command, cancellationToken);
    }

}
