using AutoHelper.Application.Common.Interfaces.Messaging.Email;
using AutoHelper.Application.Messages._DTOs;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Messaging.Interfaces;
using AutoHelper.Messaging.Models.GraphEmail;
using AutoHelper.Messaging.Templates.Conversation;
using BlazorTemplater;

namespace AutoHelper.Messaging.Services;

/// <summary>
/// https://learn.microsoft.com/en-us/graph/api/resources/message?view=graph-rest-1.0#methods
/// </summary>
internal class EmailConversationService : IEmailConversationService
{
    private readonly IEmailService _emailService;

    public EmailConversationService(IEmailService emailService)
    {
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    }

    public async Task SendMessage(ConversationMessageItem message, string senderName, CancellationToken cancellationToken)
    {
        var receiverIdentifier = message.ReceiverContactIdentifier;
        var conversationId = message.ConversationId;
        var content = message.MessageContent;

        // TODO: build html, so it looks like 1 conversation.
        // now when send an message from Whatsapp to the email
        // he will send an clean message, this will been handled in gmail as 1 message.
        // but we want an tree of responses that it looks like an conversation in gmail

        string html = new ComponentRenderer<Templates.Conversation.Message>()
            .Set(c => c.Content, content)
            .Set(c => c.ConversationId, conversationId.ToString().Split('-')[0])
            .Render();

        var email = new GraphEmail
        {
            Message = new GraphEmailMessage
            {
                Subject = $"Een bericht van {senderName}",
                Body = new GraphEmailBody
                {
                    ContentType = "HTML",
                    Content = html
                },
                From = new GraphEmailFrom
                {
                    EmailAddress = new GraphEmailAddress
                    {
                        Name = "AutoHelper",
                        Address = _emailService.GetUserId()
                    }
                },
                ToRecipients = new GraphEmailRecipient[]
                {
                    new GraphEmailRecipient()
                    {
                        EmailAddress = new GraphEmailAddress
                        {
                            Address = receiverIdentifier
                        }
                    }
                }
            }
        };

        await _emailService.SendEmail(email, cancellationToken);
    }

    public async Task SendMessageWithVehicle(ConversationMessageItem message, VehicleTechnicalDtoItem vehicle, CancellationToken cancellationToken)
    {
        var receiverIdentifier = message.ReceiverContactIdentifier;
        var conversationId = message.ConversationId;
        var content = message.MessageContent;

        string html = new ComponentRenderer<MessageWithVehicle>()
            .Set(c => c.LicensePlate, vehicle.LicensePlate)
            .Set(c => c.Content, content)
            .Set(c => c.FuelType, vehicle.FuelType)
            .Set(c => c.Model, vehicle.FullName)
            .Set(c => c.MOT, vehicle.MOTExpiryDate)
            .Set(c => c.NAP, vehicle.Mileage)
            .Set(c => c.ConversationId, conversationId.ToString().Split('-')[0])
            .Render();

        var email = new GraphEmail
        {
            Message = new GraphEmailMessage
            {
                Subject = $"Een vraag namens {vehicle.LicensePlate}",
                Body = new GraphEmailBody
                {
                    ContentType = "HTML",
                    Content = html
                },
                From = new GraphEmailFrom
                {
                    EmailAddress = new GraphEmailAddress
                    {
                        Name = "AutoHelper",
                        Address = _emailService.GetUserId()
                    }
                },
                ToRecipients = new GraphEmailRecipient[]
                {
                    new GraphEmailRecipient()
                    {
                        EmailAddress = new GraphEmailAddress
                        {
                            Address = receiverIdentifier
                        }
                    }
                }
            }
        };

        await _emailService.SendEmail(email, cancellationToken);
    }

    public async Task SendMessageConfirmation(ConversationMessageItem message, string receiverName, CancellationToken cancellationToken)
    {
        var receiverIdentifier = message.SenderContactIdentifier;
        var conversationId = message.ConversationId;
        var content = message.MessageContent;

        string html = new ComponentRenderer<MessageConfirmation>()
            .Set(c => c.ConversationId, conversationId.ToString().Split('-')[0])
            .Render();

        var email = new GraphEmail
        {
            Message = new GraphEmailMessage
            {
                Subject = $"Bericht is verstuurd naar {receiverName}",
                Body = new GraphEmailBody
                {
                    ContentType = "HTML",
                    Content = html
                },
                From = new GraphEmailFrom
                {
                    EmailAddress = new GraphEmailAddress
                    {
                        Name = "AutoHelper",
                        Address = _emailService.GetUserId()
                    }
                },
                ToRecipients = new GraphEmailRecipient[]
                {
                    new GraphEmailRecipient()
                    {
                        EmailAddress = new GraphEmailAddress
                        {
                            Address = receiverIdentifier
                        }
                    }
                }
            }
        };

        await _emailService.SendEmail(email, cancellationToken);
    }

}
