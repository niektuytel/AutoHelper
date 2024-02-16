using System.Text.Json;
using AutoHelper.Application.Common.Interfaces;
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
    private readonly IAesEncryptionService _aesEncryptionService;

    public EmailNotificationService(IEmailService emailService, IAesEncryptionService encryptionService)
    {
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _aesEncryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
    }

    public async Task SendNotification(NotificationItem notification, VehicleTechnicalDtoItem vehicle, CancellationToken cancellationToken)
    {
        switch (notification.GeneralType)
        {
            case NotificationGeneralType.VehicleServiceReview:
                await SendGarageServiceReviewReminder(notification, vehicle, cancellationToken);
                break;
            case NotificationGeneralType.VehicleServiceReviewApproved:
                await SendVehicleServiceReviewApproved(notification, vehicle, cancellationToken);
                break;
            case NotificationGeneralType.VehicleServiceReviewDeclined:
                await SendVehicleServiceReviewDeclined(notification, vehicle, cancellationToken);
                break;
            case NotificationGeneralType.VehicleServiceNotification:
                await SendVehicleServiceNotification(notification, vehicle, cancellationToken);
                break;
        }
    }

    private async Task SendGarageServiceReviewReminder(NotificationItem notification, VehicleTechnicalDtoItem vehicleInfo, CancellationToken cancellationToken)
    {
        string html = new ComponentRenderer<VehicleServiceReview>()
            .AddService(_aesEncryptionService)
            .Set(c => c.Notification, notification)
            .Set(c => c.VehicleInfo, vehicleInfo)
            .Render();

        var email = new GraphEmail
        {
            Message = new GraphEmailMessage
            {
                Subject = $"[{notification.VehicleLicensePlate}] {VehicleServiceReview.Subject}",
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

        await _emailService.SendEmail(email, cancellationToken);
    }

    private async Task SendVehicleServiceReviewApproved(NotificationItem notification, VehicleTechnicalDtoItem vehicleInfo, CancellationToken cancellationToken)
    {
        string html = new ComponentRenderer<VehicleServiceReviewApproved>()
            .Set(c => c.Notification, notification)
            .Set(c => c.VehicleInfo, vehicleInfo)
            .Render();

        var email = new GraphEmail
        {
            Message = new GraphEmailMessage
            {
                Subject = $"[{notification.VehicleLicensePlate}] {VehicleServiceReviewApproved.Subject}",
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

        await _emailService.SendEmail(email, cancellationToken);
    }

    private async Task SendVehicleServiceReviewDeclined(NotificationItem notification, VehicleTechnicalDtoItem vehicleInfo, CancellationToken cancellationToken)
    {
        string html = new ComponentRenderer<VehicleServiceReviewDeclined>()
            .Set(c => c.Notification, notification)
            .Set(c => c.VehicleInfo, vehicleInfo)
            .Render();

        var email = new GraphEmail
        {
            Message = new GraphEmailMessage
            {
                Subject = $"[{notification.VehicleLicensePlate}] {VehicleServiceReviewDeclined.Subject}",
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

        await _emailService.SendEmail(email, cancellationToken);
    }

    private async Task SendVehicleServiceNotification(NotificationItem notification, VehicleTechnicalDtoItem vehicleInfo, CancellationToken cancellationToken)
    {
        string html = "";
        string subject = "";
        switch (notification.VehicleType)
        {
            case NotificationVehicleType.MOT:
                subject = $"[{notification.VehicleLicensePlate}] {VehicleServiceNotification_MOT.Subject}";
                html = new ComponentRenderer<VehicleServiceNotification_MOT>()
                    .Set(c => c.Notification, notification)
                    .Set(c => c.VehicleInfo, vehicleInfo)
                    .Render();
                break;
            case NotificationVehicleType.WinterService:
                subject = $"[{notification.VehicleLicensePlate}] {VehicleServiceNotification_WinterService.Subject}";
                html = new ComponentRenderer<VehicleServiceNotification_WinterService>()
                    .Set(c => c.Notification, notification)
                    .Render();
                break;
            case NotificationVehicleType.ChangeToSummerTyre:
                subject = $"[{notification.VehicleLicensePlate}] {VehicleServiceNotification_TyreChangeToSummer.Subject}";
                html = new ComponentRenderer<VehicleServiceNotification_TyreChangeToSummer>()
                    .Set(c => c.Notification, notification)
                    .Render();
                break;
            case NotificationVehicleType.SummerCheck:
                subject = $"[{notification.VehicleLicensePlate}] {VehicleServiceNotification_SummerCheck.Subject}";
                html = new ComponentRenderer<VehicleServiceNotification_SummerCheck>()
                    .Set(c => c.Notification, notification)
                    .Render();
                break;
            case NotificationVehicleType.SummerService:
                subject = $"[{notification.VehicleLicensePlate}] {VehicleServiceNotification_SummerService.Subject}";
                html = new ComponentRenderer<VehicleServiceNotification_SummerService>()
                    .Set(c => c.Notification, notification)
                    .Render();
                break;
            case NotificationVehicleType.ChangeToWinterTyre:
                subject = $"[{notification.VehicleLicensePlate}] {VehicleServiceNotification_TyreChangeToWinter.Subject}";
                html = new ComponentRenderer<VehicleServiceNotification_TyreChangeToWinter>()
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

        await _emailService.SendEmail(email, cancellationToken);
    }

}
