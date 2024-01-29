using System.ComponentModel;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using AutoHelper.Application.Common.Extensions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Messages._DTOs;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Messages;
using AutoHelper.Domain.Entities.Messages.Enums;
using AutoHelper.Domain.Entities.Vehicles;
using AutoHelper.Messaging.Models.GraphEmail;
using AutoHelper.Messaging.Templates.Notification;
using AutoHelper.WebUI.Controllers;
using BlazorTemplater;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WhatsappBusiness.CloudApi;
using WhatsappBusiness.CloudApi.Exceptions;
using WhatsappBusiness.CloudApi.Interfaces;
using WhatsappBusiness.CloudApi.Messages.Requests;
using WhatsappBusiness.CloudApi.Webhook;

namespace AutoHelper.Messaging.Services;

internal class WhatsappTemplateService : IWhatsappTemplateService
{
    private readonly IWhatsAppBusinessClient _whatsAppBusinessClient;
    private readonly IWhatsappResponseService _whatsappResponseService;
    private readonly IConfiguration _configuration;
    private readonly bool _isDevelopment;
    private readonly string _developPhoneNumberId;

    public WhatsappTemplateService(IWhatsAppBusinessClient whatsAppBusinessClient, IWhatsappResponseService whatsappResponseService, IConfiguration configuration)
    {
        _whatsAppBusinessClient = whatsAppBusinessClient ?? throw new ArgumentNullException(nameof(whatsAppBusinessClient));
        _whatsappResponseService = whatsappResponseService ?? throw new ArgumentNullException(nameof(whatsappResponseService));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _isDevelopment = _configuration["Environment"] == "Development";
        _developPhoneNumberId = _configuration["WhatsApp:TestPhoneNumberId"]!;
    }

    /// <summary>
    /// https://business.facebook.com/wa/manage/template-details/?business_id=656542846083352&waba_id=107289168858080&id=859328899233016&date_range=last_30_days
    /// </summary>
    public async Task SendMessage(ConversationMessageItem message, string fromContactName, CancellationToken cancellationToken)
    {
        var receiverIdentifier = message.ReceiverContactIdentifier;
        var conversationId = message.ConversationId;
        var content = message.MessageContent;

        var phoneNumberId = GetPhoneNumberId(receiverIdentifier);
        var validatedContent = GetValidContent(content);

        var template = new TextTemplateMessageRequest
        {
            To = phoneNumberId,
            Template = new TextMessageTemplate
            {
                Name = "send_message_with_basic_content",
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
                                Text = fromContactName
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
                                Type = "TEXT",
                                Text = validatedContent
                            },
                            new TextMessageParameter
                            {
                                Type = "text",
                                Text = conversationId.ToString().Split('-')[0]
                            }
                        }
                    }
                }
            }
        };

        try
        {
            var results = await _whatsAppBusinessClient.SendTextMessageTemplateAsync(template);
            if (results != null)
            {
                await _whatsappResponseService.UpdateMessageId(message, results.Messages[0].Id, cancellationToken);
            }
        }
        catch (WhatsappBusinessCloudAPIException ex)
        {
            // TODO: Handle with ILogger or on hangfire
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    /// <summary>
    /// https://business.facebook.com/wa/manage/message-templates/?business_id=656542846083352&waba_id=107289168858080&id=2664948603645930
    /// </summary>
    public async Task SendMessageWithVehicle(ConversationMessageItem message, VehicleTechnicalDtoItem vehicle, CancellationToken cancellationToken)
    {
        var receiverIdentifier = message.ReceiverContactIdentifier;
        var conversationId = message.ConversationId;
        var content = message.MessageContent;

        var phoneNumberId = GetPhoneNumberId(receiverIdentifier);
        var template = new TextTemplateMessageRequest
        {
            To = phoneNumberId,
            Template = new TextMessageTemplate
            {
                Name = "send_message_with_vehicle_information",
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
                                Text = vehicle.LicensePlate// 87-GRN-6
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
                                Text = content// Wat is de beste prijs voor deze auto?
                            },
                            new TextMessageParameter
                            {
                                Type = "text",
                                Text = vehicle.LicensePlate// 87-GRN-6
                            },
                            new TextMessageParameter
                            {
                                Type = "text",
                                Text = vehicle.FuelType.ToTitleCase()// Benzine
                            },
                            new TextMessageParameter
                            {
                                Type = "text",
                                Text = $"{vehicle.Brand.ToTitleCase()} {vehicle.Model.ToTitleCase()}({vehicle.YearOfFirstAdmission})"// Dacia Sandero (2008)
                            },
                            new TextMessageParameter
                            {
                                Type = "text",
                                Text = vehicle.MOTExpiryDate// 10-05-2024
                            },
                            new TextMessageParameter
                            {
                                Type = "text",
                                Text = vehicle.Mileage.ToTitleCase()// Logisch
                            },
                            new TextMessageParameter
                            {
                                Type = "text",
                                Text = conversationId.ToString().Split('-')[0]// c8e7d5b8
                            },
                        }
                    },
                }
            }
        };

        try
        {
            var results = await _whatsAppBusinessClient.SendTextMessageTemplateAsync(template);
            if (results != null)
            {
                await _whatsappResponseService.UpdateMessageId(message, results.Messages[0].Id, cancellationToken);
            }
        }
        catch (WhatsappBusinessCloudAPIException ex)
        {
            // TODO: Handle with ILogger or on hangfire
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    /// <summary>
    /// https://business.facebook.com/wa/manage/message-templates/?business_id=656542846083352&waba_id=107289168858080&id=837399274834029
    /// </summary>
    public async Task SendMessageConfirmation(ConversationMessageItem message, string fromContactName, CancellationToken cancellationToken)
    {
        var receiverIdentifier = message.ReceiverContactIdentifier;
        var conversationId = message.ConversationId;

        var phoneNumberId = GetPhoneNumberId(receiverIdentifier);
        var template = new TextTemplateMessageRequest
        {
            To = phoneNumberId,
            Template = new TextMessageTemplate
            {
                Name = "send_message_with_confirmation",
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
                                Text = fromContactName
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
                                Text = conversationId.ToString().Split('-')[0]
                            }
                        }
                    }
                }
            }
        };

        try
        {
            var results = await _whatsAppBusinessClient.SendTextMessageTemplateAsync(template);
            if (results != null)
            {
                await _whatsappResponseService.UpdateMessageId(message, results.Messages[0].Id, cancellationToken);
            }
        }
        catch (WhatsappBusinessCloudAPIException ex)
        {
            // TODO: Handle with ILogger or on hangfire
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task SendNotificationMessage(NotificationItem notification, VehicleTechnicalDtoItem vehicle, CancellationToken cancellationToken)
    {
        switch (notification.GeneralType)
        {
            case GeneralNotificationType.GarageServiceReviewReminder:
                await SendGarageServiceReviewReminder(notification, vehicle, cancellationToken);
                break;
            case GeneralNotificationType.VehicleServiceReviewApproved:
                await SendVehicleServiceReviewApproved(notification, vehicle, cancellationToken);
                break;
            case GeneralNotificationType.VehicleServiceReviewDeclined:
                await SendVehicleServiceReviewDeclined(notification, vehicle, cancellationToken);
                break;
            case GeneralNotificationType.VehicleServiceNotification:
                await SendVehicleServiceNotification(notification, vehicle, cancellationToken);
                break;
        }
    }

    private async Task SendGarageServiceReviewReminder(NotificationItem notification, VehicleTechnicalDtoItem vehicle, CancellationToken cancellationToken)
    {
        var receiverIdentifier = notification.ReceiverContactIdentifier;
        var phoneNumberId = GetPhoneNumberId(receiverIdentifier);

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
        var phoneNumberId = GetPhoneNumberId(receiverIdentifier);

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
        var phoneNumberId = GetPhoneNumberId(receiverIdentifier);

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
            case VehicleNotificationType.MOT:
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
            case VehicleNotificationType.WinterService:
                //🚗 Schoonmaaktip voor uw {{2}}! 🧼
                //Na de winter is het belangrijk uw {{2}} goed schoon te maken. Dit voorkomt roest door strooizout en houdt uw auto in topstaat.
                name = "vehicle_servicenotification_winterservice";
                break;
            case VehicleNotificationType.ChangeToSummerTyre:
                name = "vehicle_servicenotification_summertyrechange";
                break;
            case VehicleNotificationType.SummerCheck:
                name = "vehicle_servicenotification_summercheck";
                break;
            case VehicleNotificationType.SummerService:
                name = "vehicle_servicenotification_summerservice2";
                break;
            case VehicleNotificationType.ChangeToWinterTyre:
                name = "vehicle_servicenotification_wintertyrechange";
                break;
        }

        var receiverIdentifier = notification.ReceiverContactIdentifier;
        var phoneNumberId = GetPhoneNumberId(receiverIdentifier);

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

    public string GetPhoneNumberId(string phoneNumber)
    {
        if (_isDevelopment)
        {
            return _developPhoneNumberId;
        }

        phoneNumber = phoneNumber
            .Replace(" ", "")
            .Replace("-", "")
            .Replace("(", "")
            .Replace(")", "")
            .Replace("+", "");

        // Removing any leading "0" and adding "31" (Netherlands country code) if not present
        if (phoneNumber.StartsWith("0"))
            phoneNumber = "31" + phoneNumber[1..];
        else if (!phoneNumber.StartsWith("31"))
            phoneNumber = "31" + phoneNumber;

        return phoneNumber;
    }

    /// <summary>
    /// Remove html when message is html
    /// </summary>
    private string GetValidContent(string content)
    {
        var autohelperEmail = _configuration["GraphMicrosoft:UserId"]?.ToString() ?? "";

        // Regex pattern to extract the content between <div> tags
        string pattern = @"<div(.*?)>(.*?)<\/div>";

        // Find matches
        var matches = Regex.Matches(content, pattern, RegexOptions.Singleline);
        if (matches.Count == 0)
        {
            return content;
        }

        var message = string.Empty;
        foreach (Match match in matches)
        {
            if (match.ToString().Contains(autohelperEmail))
            {
                break;
            }

            message += match.ToString();
        }

        // Fix all html encoded spaces
        message = Regex.Replace(message, @"&nbsp;", " ");

        // Replace <br> and <br /> with '   ' as this can mostly respond to an ending of an line
        message = Regex.Replace(message, @"<br\s?\/?>", "  ");

        // Remove all div tags
        message = Regex.Replace(message, @"<(.*?)div(.*?)>", "");

        // Remove all span tags
        message = Regex.Replace(message, @"<(.*?)span(.*?)>", "");

        // Remove all p tags
        message = Regex.Replace(message, @"<(.*?)p(.*?)>", "");

        // Remove all strong tags
        message = Regex.Replace(message, @"<(.*?)strong(.*?)>", "");

        // Remove all a tags
        message = Regex.Replace(message, @"<(.*?)a(.*?)>", "");

        // Remove all li tags
        message = Regex.Replace(message, @"<(.*?)li(.*?)>", "");

        // Remove all ul tags
        message = Regex.Replace(message, @"<(.*?)ul(.*?)>", "");

        // Remove all ol tags
        message = Regex.Replace(message, @"<(.*?)ol(.*?)>", "");

        // Remove all img tags
        message = Regex.Replace(message, @"<(.*?)img(.*?)>", "");

        return message;
    }

}