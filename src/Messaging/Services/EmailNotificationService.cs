using System.Text.Json;
using AutoHelper.Application.Common.Interfaces.Messaging.Email;
using AutoHelper.Application.Messages._DTOs;
using AutoHelper.Domain.Entities.Communication;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Messages;
using AutoHelper.Messaging.Interfaces;
using AutoHelper.Messaging.Models;
using AutoHelper.Messaging.Models.GraphEmail;
using AutoHelper.Messaging.Templates.Conversation;
using AutoHelper.Messaging.Templates.Notification;
using BlazorTemplater;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace AutoHelper.Messaging.Services;

/// <summary>
/// https://learn.microsoft.com/en-us/graph/api/resources/message?view=graph-rest-1.0#methods
/// </summary>
internal class EmailNotificationService : IEmailNotificationService
{
    private readonly IEmailService _emailService;

    public EmailNotificationService(IEmailService emailService)
    {
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    }

    public async Task SendNotification(NotificationItem notification, VehicleTechnicalDtoItem vehicle, CancellationToken cancellationToken)
    {
        switch (notification.GeneralType)
        {
            case NotificationGeneralType.GarageServiceReviewReminder:
                await SendGarageServiceReviewReminder(notification, cancellationToken);
                break;
            case NotificationGeneralType.VehicleServiceReviewApproved:
                await SendVehicleServiceReviewApproved(notification, cancellationToken);
                break;
            case NotificationGeneralType.VehicleServiceReviewDeclined:
                await SendVehicleServiceReviewDeclined(notification, cancellationToken);
                break;
            case NotificationGeneralType.VehicleServiceNotification:
                await SendVehicleServiceNotification(notification, cancellationToken);
                break;
        }
    }

    private async Task SendGarageServiceReviewReminder(NotificationItem notification, CancellationToken cancellationToken)
    {
        string html = new ComponentRenderer<GarageServiceReviewReminder>()
            .Set(c => c.Notification, notification)
            .Render();

        var email = new GraphEmail
        {
            Message = new GraphEmailMessage
            {
                Subject = $"Bevestiging gevraagd voor onderhoud aan ${notification.VehicleLicensePlate}",
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
                            Address = notification.ReceiverContactIdentifier
                        }
                    }
                }
            }
        };

        await _emailService.SendEmail(email);
    }

    private async Task SendVehicleServiceReviewApproved(NotificationItem notification, CancellationToken cancellationToken)
    {
        string html = new ComponentRenderer<VehicleServiceReviewApproved>()
            .Set(c => c.Notification, notification)
            .Render();

        var email = new GraphEmail
        {
            Message = new GraphEmailMessage
            {
                Subject = $"Onderhoudsregel Goedgekeurd voor [{notification.VehicleLicensePlate}]: Bevestiging van Garage",
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
                            Address = notification.ReceiverContactIdentifier
                        }
                    }
                }
            }
        };

        await _emailService.SendEmail(email);
    }

    private async Task SendVehicleServiceReviewDeclined(NotificationItem notification, CancellationToken cancellationToken)
    {
        string html = new ComponentRenderer<VehicleServiceReviewDeclined>()
            .Set(c => c.Notification, notification)
            .Render();

        var email = new GraphEmail
        {
            Message = new GraphEmailMessage
            {
                Subject = $"Onderhoudsregel afgekeurd voor [{notification.VehicleLicensePlate}]: Bevestiging van Garage",
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
                            Address = notification.ReceiverContactIdentifier
                        }
                    }
                }
            }
        };

        await _emailService.SendEmail(email);
    }

    private async Task SendVehicleServiceNotification(NotificationItem notification, CancellationToken cancellationToken)
    {
        string html = "";
        string subject = "";
        switch (notification.VehicleType)
        {
            case NotificationVehicleType.MOT:
                subject = $"APK verloopt over 4 weken voor [{notification.VehicleLicensePlate}]";
                html = new ComponentRenderer<VehicleServiceNotification_MOT>()
                    .Set(c => c.Notification, notification)
                    .Render();
                break;
            case NotificationVehicleType.WinterService:
                subject = $"Zorg goed voor uw auto: Overweeg een onderhoudsbeurt na een intensieve winterperiode [{notification.VehicleLicensePlate}]";
                html = new ComponentRenderer<VehicleServiceNotification_WinterService>()
                    .Set(c => c.Notification, notification)
                    .Render();
                break;
            case NotificationVehicleType.ChangeToSummerTyre:
                subject = $"Tijd om uw Winterbanden te Wisselen voor de Zomer: [{notification.VehicleLicensePlate}]";
                html = new ComponentRenderer<VehicleServiceNotification_SummerTyreChange>()
                    .Set(c => c.Notification, notification)
                    .Render();
                break;
            case NotificationVehicleType.SummerCheck:
                subject = $"Is uw auto klaar voor de vakantie? Plan een Zomercheck voor [{notification.VehicleLicensePlate}]";
                html = new ComponentRenderer<VehicleServiceNotification_SummerCheck>()
                    .Set(c => c.Notification, notification)
                    .Render();
                break;
            case NotificationVehicleType.SummerService:
                subject = $"Heeft u een vakantietrip gemaakt? Overweeg een onderhoudsbeurt voor uw auto [{notification.VehicleLicensePlate}]";
                html = new ComponentRenderer<VehicleServiceNotification_SummerService>()
                    .Set(c => c.Notification, notification)
                    .Render();
                break;
            case NotificationVehicleType.ChangeToWinterTyre:
                subject = $"Bereid uw auto voor op de winter: Tijd voor winterbanden [{notification.VehicleLicensePlate}]";
                html = new ComponentRenderer<VehicleServiceNotification_WinterTyreChange>()
                    .Set(c => c.Notification, notification)
                    .Render();
                break;
        }

        var email = new GraphEmail
        {
            Message = new GraphEmailMessage
            {
                Subject = subject,
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
                            Address = notification.ReceiverContactIdentifier
                        }
                    }
                }
            }
        };

        await _emailService.SendEmail(email);
    }

}
