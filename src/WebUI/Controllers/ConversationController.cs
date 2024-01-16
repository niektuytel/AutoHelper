using System.Text;
using AutoHelper.Application.Common.Extensions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Conversations.Commands.CreateConversationMessage;
using AutoHelper.Application.Conversations.Commands.CreateGarageConversationItems;
using AutoHelper.Application.Conversations.Commands.ReceiveMessage;
using AutoHelper.Application.Conversations.Commands.StartConversationItems;
using AutoHelper.Hangfire.MediatR;
using Hangfire;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using WebUI.Models;

namespace AutoHelper.WebUI.Controllers;

public class ConversationController : ApiControllerBase
{
    const string VerifyToken = "Autohelper";
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly ILogger<ConversationController> _logger;

    public ConversationController(IBackgroundJobClient backgroundJobClient, ILogger<ConversationController> logger)
    {
        _backgroundJobClient = backgroundJobClient;
        _logger = logger;
    }

    [HttpGet(nameof(ConfigureWhatsappWebhook))]
    public ActionResult<string> ConfigureWhatsappWebhook(
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

    [HttpPost($"{nameof(ReceiveEmailMessage)}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    public async Task<string?> ReceiveEmailMessage([FromBody] ReceiveMessageCommand message, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(message, cancellationToken);
        return result;
    }

    [HttpPost(nameof(ReceiveWhatsappMessage))]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReceiveWhatsappMessage([FromBody] dynamic message)
    {
        throw new NotImplementedException();
        //try
        //{
        //    if (message is null)
        //    {
        //        return BadRequest(new
        //        {
        //            Message = "Message not received"
        //        });
        //    }

        //    // Message status updates will be trigerred in different scenario
        //    var changesResult = message["entry"][0]["changes"][0]["value"];

        //    if (changesResult["statuses"] != null)
        //    {
        //        var messageStatus = Convert.ToString(message["entry"][0]["changes"][0]["value"]["statuses"][0]["status"]);

        //        if (messageStatus.Equals("sent"))
        //        {
        //            var messageStatusReceived = JsonConvert.DeserializeObject<UserInitiatedMessageSentStatus>(Convert.ToString(message)) as UserInitiatedMessageSentStatus;
        //            var messageStatusResults = new List<UserInitiatedStatus>(messageStatusReceived.Entry.SelectMany(x => x.Changes).SelectMany(x => x.Value.Statuses));
        //            _logger.LogInformation(JsonConvert.SerializeObject(messageStatusResults, Formatting.Indented));

        //            return Ok(new
        //            {
        //                Message = $"Message Status Received: {messageStatus}"
        //            });
        //        }

        //        if (messageStatus.Equals("delivered"))
        //        {
        //            var messageStatusReceived = JsonConvert.DeserializeObject<UserInitiatedMessageDeliveredStatus>(Convert.ToString(message)) as UserInitiatedMessageDeliveredStatus;
        //            var messageStatusResults = new List<UserInitiatedMessageDeliveryStatus>(messageStatusReceived.Entry.SelectMany(x => x.Changes).SelectMany(x => x.Value.Statuses));
        //            _logger.LogInformation(JsonConvert.SerializeObject(messageStatusResults, Formatting.Indented));

        //            return Ok(new
        //            {
        //                Message = $"Message Status Received: {messageStatus}"
        //            });
        //        }

        //        if (messageStatus.Equals("read"))
        //        {
        //            return Ok(new
        //            {
        //                Message = $"Message Status Received: {messageStatus}"
        //            });
        //        }
        //    }
        //    else
        //    {
        //        var messageType = Convert.ToString(message["entry"][0]["changes"][0]["value"]["messages"][0]["type"]);

        //        if (messageType.Equals("text"))
        //        {
        //            var textMessageReceived = JsonConvert.DeserializeObject<TextMessageReceived>(Convert.ToString(message)) as TextMessageReceived;
        //            textMessage = new List<TextMessage>(textMessageReceived.Entry.SelectMany(x => x.Changes).SelectMany(x => x.Value.Messages));
        //            _logger.LogInformation(JsonConvert.SerializeObject(textMessage, Formatting.Indented));

        //            MarkMessageRequest markMessageRequest = new MarkMessageRequest();
        //            markMessageRequest.MessageId = textMessage.SingleOrDefault().Id;
        //            markMessageRequest.Status = "read";

        //            await _whatsAppBusinessClient.MarkMessageAsReadAsync(markMessageRequest);

        //            TextMessageReplyRequest textMessageReplyRequest = new TextMessageReplyRequest();
        //            textMessageReplyRequest.Context = new WhatsappBusiness.CloudApi.Messages.ReplyRequests.TextMessageContext();
        //            textMessageReplyRequest.Context.MessageId = textMessage.SingleOrDefault().Id;
        //            textMessageReplyRequest.To = textMessage.SingleOrDefault().From;
        //            textMessageReplyRequest.Text = new WhatsAppText();
        //            textMessageReplyRequest.Text.Body = "Your Message was received. Processing the request shortly";
        //            textMessageReplyRequest.Text.PreviewUrl = false;

        //            await _whatsAppBusinessClient.SendTextMessageAsync(textMessageReplyRequest);

        //            return Ok(new
        //            {
        //                Message = "Text Message received"
        //            });
        //        }

        //        if (messageType.Equals("image"))
        //        {
        //            var imageMessageReceived = JsonConvert.DeserializeObject<ImageMessageReceived>(Convert.ToString(message)) as ImageMessageReceived;
        //            imageMessage = new List<ImageMessage>(imageMessageReceived.Entry.SelectMany(x => x.Changes).SelectMany(x => x.Value.Messages));
        //            _logger.LogInformation(JsonConvert.SerializeObject(imageMessage, Formatting.Indented));

        //            MarkMessageRequest markMessageRequest = new MarkMessageRequest();
        //            markMessageRequest.MessageId = imageMessage.SingleOrDefault().Id;
        //            markMessageRequest.Status = "read";

        //            await _whatsAppBusinessClient.MarkMessageAsReadAsync(markMessageRequest);

        //            return Ok(new
        //            {
        //                Message = "Image Message received"
        //            });
        //        }

        //        if (messageType.Equals("audio"))
        //        {
        //            var audioMessageReceived = JsonConvert.DeserializeObject<AudioMessageReceived>(Convert.ToString(message)) as AudioMessageReceived;
        //            audioMessage = new List<AudioMessage>(audioMessageReceived.Entry.SelectMany(x => x.Changes).SelectMany(x => x.Value.Messages));
        //            _logger.LogInformation(JsonConvert.SerializeObject(audioMessage, Formatting.Indented));

        //            MarkMessageRequest markMessageRequest = new MarkMessageRequest();
        //            markMessageRequest.MessageId = audioMessage.SingleOrDefault().Id;
        //            markMessageRequest.Status = "read";

        //            await _whatsAppBusinessClient.MarkMessageAsReadAsync(markMessageRequest);

        //            var mediaUrlResponse = await _whatsAppBusinessClient.GetMediaUrlAsync(audioMessage.SingleOrDefault().Audio.Id);

        //            _logger.LogInformation(mediaUrlResponse.Url);

        //            // To download media received sent by user
        //            var mediaFileDownloaded = await _whatsAppBusinessClient.DownloadMediaAsync(mediaUrlResponse.Url);

        //            var rootPath = Path.Combine(_webHostEnvironment.WebRootPath, "Application_Files\\MediaDownloads\\");

        //            if (!Directory.Exists(rootPath))
        //            {
        //                Directory.CreateDirectory(rootPath);
        //            }

        //            // Get the path of filename
        //            string filename = string.Empty;

        //            if (mediaUrlResponse.MimeType.Contains("audio/mpeg"))
        //            {
        //                filename = $"{mediaUrlResponse.Id}.mp3";
        //            }

        //            if (mediaUrlResponse.MimeType.Contains("audio/ogg"))
        //            {
        //                filename = $"{mediaUrlResponse.Id}.ogg";
        //            }

        //            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Application_Files\\MediaDownloads\\", filename);

        //            await System.IO.File.WriteAllBytesAsync(filePath, mediaFileDownloaded);

        //            return Ok(new
        //            {
        //                Message = "Audio Message received"
        //            });
        //        }

        //        if (messageType.Equals("document"))
        //        {
        //            var documentMessageReceived = JsonConvert.DeserializeObject<DocumentMessageReceived>(Convert.ToString(message)) as DocumentMessageReceived;
        //            documentMessage = new List<DocumentMessage>(documentMessageReceived.Entry.SelectMany(x => x.Changes).SelectMany(x => x.Value.Messages));
        //            _logger.LogInformation(JsonConvert.SerializeObject(documentMessage, Formatting.Indented));

        //            MarkMessageRequest markMessageRequest = new MarkMessageRequest();
        //            markMessageRequest.MessageId = documentMessage.SingleOrDefault().Id;
        //            markMessageRequest.Status = "read";

        //            await _whatsAppBusinessClient.MarkMessageAsReadAsync(markMessageRequest);

        //            var mediaUrlResponse = await _whatsAppBusinessClient.GetMediaUrlAsync(documentMessage.SingleOrDefault().Document.Id);

        //            _logger.LogInformation(mediaUrlResponse.Url);

        //            // To download media received sent by user
        //            var mediaFileDownloaded = await _whatsAppBusinessClient.DownloadMediaAsync(mediaUrlResponse.Url);

        //            var rootPath = Path.Combine(_webHostEnvironment.WebRootPath, "Application_Files\\MediaDownloads\\");

        //            if (!Directory.Exists(rootPath))
        //            {
        //                Directory.CreateDirectory(rootPath);
        //            }

        //            // Get the path of filename
        //            string filename = string.Empty;

        //            if (mediaUrlResponse.MimeType.Contains("audio/mpeg"))
        //            {
        //                filename = $"{mediaUrlResponse.Id}.mp3";
        //            }

        //            if (mediaUrlResponse.MimeType.Contains("audio/ogg"))
        //            {
        //                filename = $"{mediaUrlResponse.Id}.ogg";
        //            }

        //            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Application_Files\\MediaDownloads\\", filename);

        //            await System.IO.File.WriteAllBytesAsync(filePath, mediaFileDownloaded);

        //            return Ok(new
        //            {
        //                Message = "Document Message received"
        //            });
        //        }

        //        if (messageType.Equals("sticker"))
        //        {
        //            var stickerMessageReceived = JsonConvert.DeserializeObject<StickerMessageReceived>(Convert.ToString(message)) as StickerMessageReceived;
        //            stickerMessage = new List<StickerMessage>(stickerMessageReceived.Entry.SelectMany(x => x.Changes).SelectMany(x => x.Value.Messages));
        //            _logger.LogInformation(JsonConvert.SerializeObject(imageMessage, Formatting.Indented));

        //            MarkMessageRequest markMessageRequest = new MarkMessageRequest();
        //            markMessageRequest.MessageId = stickerMessage.SingleOrDefault().Id;
        //            markMessageRequest.Status = "read";

        //            await _whatsAppBusinessClient.MarkMessageAsReadAsync(markMessageRequest);

        //            return Ok(new
        //            {
        //                Message = "Image Message received"
        //            });
        //        }

        //        if (messageType.Equals("contacts"))
        //        {
        //            var contactMessageReceived = JsonConvert.DeserializeObject<ContactMessageReceived>(Convert.ToString(message)) as ContactMessageReceived;
        //            contactMessage = new List<ContactMessage>(contactMessageReceived.Entry.SelectMany(x => x.Changes).SelectMany(x => x.Value.Messages));
        //            _logger.LogInformation(JsonConvert.SerializeObject(contactMessage, Formatting.Indented));

        //            MarkMessageRequest markMessageRequest = new MarkMessageRequest();
        //            markMessageRequest.MessageId = contactMessage.SingleOrDefault().Id;
        //            markMessageRequest.Status = "read";

        //            await _whatsAppBusinessClient.MarkMessageAsReadAsync(markMessageRequest);

        //            return Ok(new
        //            {
        //                Message = "Contact Message Received"
        //            });
        //        }


        //        if (messageType.Equals("location"))
        //        {
        //            var locationMessageReceived = JsonConvert.DeserializeObject<StaticLocationMessageReceived>(Convert.ToString(message)) as StaticLocationMessageReceived;
        //            locationMessage = new List<LocationMessage>(locationMessageReceived.Entry.SelectMany(x => x.Changes).SelectMany(x => x.Value.Messages));
        //            _logger.LogInformation(JsonConvert.SerializeObject(locationMessage, Formatting.Indented));

        //            MarkMessageRequest markMessageRequest = new MarkMessageRequest();
        //            markMessageRequest.MessageId = locationMessage.SingleOrDefault().Id;
        //            markMessageRequest.Status = "read";

        //            await _whatsAppBusinessClient.MarkMessageAsReadAsync(markMessageRequest);

        //            LocationMessageReplyRequest locationMessageReplyRequest = new LocationMessageReplyRequest();
        //            locationMessageReplyRequest.Context = new WhatsappBusiness.CloudApi.Messages.ReplyRequests.LocationMessageContext();
        //            locationMessageReplyRequest.Context.MessageId = locationMessage.SingleOrDefault().Id;
        //            locationMessageReplyRequest.To = locationMessage.SingleOrDefault().From;
        //            locationMessageReplyRequest.Location = new WhatsappBusiness.CloudApi.Messages.Requests.Location();
        //            locationMessageReplyRequest.Location.Name = "Location Test";
        //            locationMessageReplyRequest.Location.Address = "Address Test";
        //            locationMessageReplyRequest.Location.Longitude = -122.425332;
        //            locationMessageReplyRequest.Location.Latitude = 37.758056;

        //            await _whatsAppBusinessClient.SendLocationMessageAsync(locationMessageReplyRequest);

        //            return Ok(new
        //            {
        //                Message = "Location Message Received"
        //            });
        //        }

        //        if (messageType.Equals("button"))
        //        {
        //            var quickReplyMessageReceived = JsonConvert.DeserializeObject<QuickReplyButtonMessageReceived>(Convert.ToString(message)) as QuickReplyButtonMessageReceived;
        //            quickReplyButtonMessage = new List<QuickReplyButtonMessage>(quickReplyMessageReceived.Entry.SelectMany(x => x.Changes).SelectMany(x => x.Value.Messages));
        //            _logger.LogInformation(JsonConvert.SerializeObject(quickReplyButtonMessage, Formatting.Indented));

        //            MarkMessageRequest markMessageRequest = new MarkMessageRequest();
        //            markMessageRequest.MessageId = quickReplyButtonMessage.SingleOrDefault().Id;
        //            markMessageRequest.Status = "read";

        //            await _whatsAppBusinessClient.MarkMessageAsReadAsync(markMessageRequest);

        //            return Ok(new
        //            {
        //                Message = "Quick Reply Button Message Received"
        //            });
        //        }

        //        if (messageType.Equals("interactive"))
        //        {
        //            var getInteractiveType = Convert.ToString(message["entry"][0]["changes"][0]["value"]["messages"][0]["interactive"]["type"]);

        //            if (getInteractiveType.Equals("button_reply"))
        //            {
        //                var replyMessageReceived = JsonConvert.DeserializeObject<ReplyButtonMessageReceived>(Convert.ToString(message)) as ReplyButtonMessageReceived;
        //                replyButtonMessage = new List<ReplyButtonMessage>(replyMessageReceived.Entry.SelectMany(x => x.Changes).SelectMany(x => x.Value.Messages));
        //                _logger.LogInformation(JsonConvert.SerializeObject(replyButtonMessage, Formatting.Indented));

        //                MarkMessageRequest markMessageRequest = new MarkMessageRequest();
        //                markMessageRequest.MessageId = replyButtonMessage.SingleOrDefault().Id;
        //                markMessageRequest.Status = "read";

        //                await _whatsAppBusinessClient.MarkMessageAsReadAsync(markMessageRequest);

        //                return Ok(new
        //                {
        //                    Message = "Reply Button Message Received"
        //                });
        //            }

        //            if (getInteractiveType.Equals("list_reply"))
        //            {
        //                var listReplyMessageReceived = JsonConvert.DeserializeObject<ListReplyButtonMessageReceived>(Convert.ToString(message)) as ListReplyButtonMessageReceived;
        //                listReplyButtonMessage = new List<ListReplyButtonMessage>(listReplyMessageReceived.Entry.SelectMany(x => x.Changes).SelectMany(x => x.Value.Messages));
        //                _logger.LogInformation(JsonConvert.SerializeObject(listReplyButtonMessage, Formatting.Indented));

        //                MarkMessageRequest markMessageRequest = new MarkMessageRequest();
        //                markMessageRequest.MessageId = listReplyButtonMessage.SingleOrDefault().Id;
        //                markMessageRequest.Status = "read";

        //                await _whatsAppBusinessClient.MarkMessageAsReadAsync(markMessageRequest);

        //                return Ok(new
        //                {
        //                    Message = "List Reply Message Received"
        //                });
        //            }
        //        }
        //    }
        //    return Ok();
        //}
        //catch (WhatsappBusinessCloudAPIException ex)
        //{
        //    _logger.LogError(ex, ex.Message);
        //    return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        //}
    }

    [HttpPost($"{nameof(StartGarageConversation)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StartGarageConversation([FromBody] CreateGarageConversationItemsCommand command, CancellationToken cancellationToken)
    {
        var conversations = await Mediator.Send(command, cancellationToken);
        var conversationIds = conversations.Select(x => x.Id).ToList();

        var sender = command.UserEmailAddress;
        if(string.IsNullOrWhiteSpace(command.UserEmailAddress))
        {
            sender = command.UserWhatsappNumber;
        }

        var queue = nameof(StartConversationItemsCommand);
        var title = $"[{sender}]: {command.MessageType.ToString()}";
        var startConversationItemsCommand = new StartConversationItemsCommand(conversationIds);
        Mediator.Enqueue(_backgroundJobClient, queue, title, startConversationItemsCommand);

        return Ok();
    }

}
