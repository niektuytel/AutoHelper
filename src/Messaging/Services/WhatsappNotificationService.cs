using System.Text.RegularExpressions;
using AutoHelper.Application.Common.Extensions;
using AutoHelper.Application.Common.Interfaces.Messaging.Whatsapp;
using AutoHelper.Application.Messages._DTOs;
using AutoHelper.Domain.Entities.Communication;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Messages;
using AutoHelper.Messaging.Interfaces;
using Microsoft.Extensions.Configuration;
using WhatsappBusiness.CloudApi;
using WhatsappBusiness.CloudApi.Exceptions;
using WhatsappBusiness.CloudApi.Interfaces;
using WhatsappBusiness.CloudApi.Messages.Requests;

namespace AutoHelper.Messaging.Services;

internal class WhatsappNotificationService : IWhatsappNotificationService
{
    private readonly IWhatsAppBusinessClient _whatsAppBusinessClient;
    private readonly IWhatsappService _whatsappService;

    public WhatsappNotificationService(IWhatsAppBusinessClient whatsAppBusinessClient, IWhatsappService whatsappService)
    {
        _whatsAppBusinessClient = whatsAppBusinessClient ?? throw new ArgumentNullException(nameof(whatsAppBusinessClient));
        _whatsappService = whatsappService ?? throw new ArgumentNullException(nameof(whatsappService));
    }

    public async Task SendNotification(NotificationItem notification, VehicleTechnicalDtoItem vehicle, CancellationToken cancellationToken)
    {
        switch (notification.GeneralType)
        {
            case NotificationGeneralType.GarageServiceReviewReminder:
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

    private async Task SendGarageServiceReviewReminder(NotificationItem notification, VehicleTechnicalDtoItem vehicle, CancellationToken cancellationToken)
    {
        var receiverIdentifier = notification.ReceiverContactIdentifier;
        var phoneNumberId = _whatsappService.GetPhoneNumberId(receiverIdentifier);

        var template = new TextTemplateMessageRequest
        {
            To = phoneNumberId,
            Template = new TextMessageTemplate
            {
                Name = "garage_servicereview_reminder",
                Language = new TextMessageLanguage
                {
                    Code = LanguageCode.Dutch
                },
                Components = new List<TextMessageComponent>
                {
                    new TextMessageComponent
                    {
                        Type = "Header",
                        Parameters = new List<TextMessageParameter>
                        {
                            new TextMessageParameter
                            {
                                Type = "text",
                                Text = notification.VehicleLicensePlate
                            }
                        }
                    },
                    new TextMessageComponent
                    {
                        Type = "Body",
                        Parameters = new List<TextMessageParameter>
                        {
                            new TextMessageParameter
                            {
                                Type = "text",
                                Text = notification.Metadata["description"]
                            },
                            new TextMessageParameter
                            {
                                Type = "text",
                                Text = notification.VehicleLicensePlate
                            },
                            new TextMessageParameter
                            {
                                Type = "text",
                                Text = vehicle.Model
                            },
                            new TextMessageParameter
                            {
                                Type = "text",
                                Text = vehicle.FuelType
                            }
                        }
                    },
                    new TextMessageComponent()
                    {
                        Type = "button",
                        Parameters = new List<TextMessageParameter>()
                        {
                            new TextMessageParameter()
                            {
                                Type = "text",
                                Text = notification.Metadata["serviceLogId"]
                            }
                        }
                    }
                }
            }
        };

        try
        {
            var results = await _whatsAppBusinessClient.SendTextMessageTemplateAsync(template);
            // NO need to update message id
        }
        catch (WhatsappBusinessCloudAPIException ex)
        {
            // TODO: Handle with ILogger or on hangfire
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    private async Task SendVehicleServiceReviewApproved(NotificationItem notification, VehicleTechnicalDtoItem vehicle, CancellationToken cancellationToken)
    {
        var receiverIdentifier = notification.ReceiverContactIdentifier;
        var phoneNumberId = _whatsappService.GetPhoneNumberId(receiverIdentifier);

        var template = new TextTemplateMessageRequest
        {
            To = phoneNumberId,
            Template = new TextMessageTemplate
            {
                Name = "vehicle_servicereview_approved",
                Language = new TextMessageLanguage
                {
                    Code = LanguageCode.Dutch
                },
                Components = new List<TextMessageComponent>
                {
                    new TextMessageComponent
                    {
                        Type = "Body",
                        Parameters = new List<TextMessageParameter>
                        {
                            new TextMessageParameter
                            {
                                Type = "text",
                                Text = notification.VehicleLicensePlate
                            },
                            new TextMessageParameter
                            {
                                Type = "text",
                                Text = vehicle.Model
                            },
                        }
                    }
                }
            }
        };

        try
        {
            var results = await _whatsAppBusinessClient.SendTextMessageTemplateAsync(template);
            // NO need to update message id
        }
        catch (WhatsappBusinessCloudAPIException ex)
        {
            // TODO: Handle with ILogger or on hangfire
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    private async Task SendVehicleServiceReviewDeclined(NotificationItem notification, VehicleTechnicalDtoItem vehicle, CancellationToken cancellationToken)
    {
        var receiverIdentifier = notification.ReceiverContactIdentifier;
        var phoneNumberId = _whatsappService.GetPhoneNumberId(receiverIdentifier);

        var template = new TextTemplateMessageRequest
        {
            To = phoneNumberId,
            Template = new TextMessageTemplate
            {
                Name = "vehicle_servicereview_declined",
                Language = new TextMessageLanguage
                {
                    Code = LanguageCode.Dutch
                },
                Components = new List<TextMessageComponent>
                {
                    new TextMessageComponent
                    {
                        Type = "Body",
                        Parameters = new List<TextMessageParameter>
                        {
                            new TextMessageParameter
                            {
                                Type = "text",
                                Text = notification.VehicleLicensePlate
                            },
                            new TextMessageParameter
                            {
                                Type = "text",
                                Text = vehicle.Model
                            },
                        }
                    }
                }
            }
        };

        try
        {
            var results = await _whatsAppBusinessClient.SendTextMessageTemplateAsync(template);
            // NO need to update message id
        }
        catch (WhatsappBusinessCloudAPIException ex)
        {
            // TODO: Handle with ILogger or on hangfire
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    private async Task SendVehicleServiceNotification(NotificationItem notification, VehicleTechnicalDtoItem vehicle, CancellationToken cancellationToken)
    {
        var name = "";
        var components = new List<TextMessageComponent>
        {
            new TextMessageComponent
            {
                Type = "Body",
                Parameters = new List<TextMessageParameter>
                {
                    new TextMessageParameter
                    {
                        Type = "text",
                        Text = notification.VehicleLicensePlate
                    },
                    new TextMessageParameter
                    {
                        Type = "text",
                        Text = vehicle.Model
                    },
                }
            }
        };

        switch (notification.VehicleType)
        {
            case NotificationVehicleType.MOT:
                name = "vehicle_servicenotification_mot";
                components = new List<TextMessageComponent>
                {
                    new TextMessageComponent
                    {
                        Type = "Header",
                        Parameters = new List<TextMessageParameter>
                        {
                            new TextMessageParameter
                            {
                                Type = "text",
                                Text = notification.VehicleLicensePlate
                            }
                        }
                    },
                    new TextMessageComponent
                    {
                        Type = "Body",
                        Parameters = new List<TextMessageParameter>
                        {
                            new TextMessageParameter
                            {
                                Type = "text",
                                Text = $"{vehicle.Brand} {vehicle.Model}"
                            },
                            new TextMessageParameter
                            {
                                Type = "text",
                                Text = vehicle.MOTExpiryDate
                            },
                        }
                    }
                };
                break;
            case NotificationVehicleType.WinterService:
                name = "vehicle_servicenotification_winterservice";
                break;
            case NotificationVehicleType.ChangeToSummerTyre:
                name = "vehicle_servicenotification_summertyrechange";
                break;
            case NotificationVehicleType.SummerCheck:
                name = "vehicle_servicenotification_summercheck";
                break;
            case NotificationVehicleType.SummerService:
                name = "vehicle_servicenotification_summerservice";
                break;
            case NotificationVehicleType.ChangeToWinterTyre:
                name = "vehicle_servicenotification_wintertyrechange";
                break;
        }

        var receiverIdentifier = notification.ReceiverContactIdentifier;
        var phoneNumberId = _whatsappService.GetPhoneNumberId(receiverIdentifier);

        var template = new TextTemplateMessageRequest
        {
            To = phoneNumberId,
            Template = new TextMessageTemplate
            {
                Name = name,
                Language = new TextMessageLanguage
                {
                    Code = LanguageCode.Dutch
                },
                Components = components
            }
        };

        try
        {
            var results = await _whatsAppBusinessClient.SendTextMessageTemplateAsync(template);
            // NO need to update message id
        }
        catch (WhatsappBusinessCloudAPIException ex)
        {
            // TODO: Handle with ILogger or on hangfire
            Console.WriteLine(ex.Message);
            throw;
        }
    }

}