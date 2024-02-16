using AutoHelper.Application.Common.Extensions;
using AutoHelper.Application.Common.Interfaces.Messaging;
using AutoHelper.Application.Common.Interfaces.Messaging.Whatsapp;
using AutoHelper.Application.Messages._DTOs;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Messaging.Interfaces;
using WhatsappBusiness.CloudApi;
using WhatsappBusiness.CloudApi.Exceptions;
using WhatsappBusiness.CloudApi.Interfaces;
using WhatsappBusiness.CloudApi.Messages.Requests;

namespace AutoHelper.Messaging.Services;

internal class WhatsappConversationService : IWhatsappConversationService
{
    private readonly IWhatsAppBusinessClient _whatsAppBusinessClient;
    private readonly IWhatsappResponseService _whatsappResponseService;
    private readonly IWhatsappService _whatsappService;

    public WhatsappConversationService(IWhatsAppBusinessClient whatsAppBusinessClient, IWhatsappResponseService whatsappResponseService, IWhatsappService whatsappService)
    {
        _whatsAppBusinessClient = whatsAppBusinessClient ?? throw new ArgumentNullException(nameof(whatsAppBusinessClient));
        _whatsappResponseService = whatsappResponseService ?? throw new ArgumentNullException(nameof(whatsappResponseService));
        _whatsappService = whatsappService ?? throw new ArgumentNullException(nameof(whatsappService));
    }

    /// <summary>
    /// https://business.facebook.com/wa/manage/template-details/?business_id=656542846083352&waba_id=107289168858080&id=859328899233016&date_range=last_30_days
    /// </summary>
    public async Task SendMessage(ConversationMessageItem message, string fromContactName, CancellationToken cancellationToken)
    {
        var receiverIdentifier = message.ReceiverContactIdentifier;
        var conversationId = message.ConversationId;
        var content = message.MessageContent;

        var phoneNumberId = _whatsappService.GetPhoneNumberId(receiverIdentifier);
        var validatedContent = _whatsappService.GetValidContent(content);

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

        var phoneNumberId = _whatsappService.GetPhoneNumberId(receiverIdentifier);
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
                                Text = vehicle.FullName// Dacia Sandero (2008)
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

        var phoneNumberId = _whatsappService.GetPhoneNumberId(receiverIdentifier);
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

}