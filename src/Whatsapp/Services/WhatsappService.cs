using System.Net.Http;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Domain.Entities.Vehicles;
using Microsoft.Extensions.Logging;
using WhatsappBusiness.CloudApi;
using WhatsappBusiness.CloudApi.Exceptions;
using WhatsappBusiness.CloudApi.Interfaces;
using WhatsappBusiness.CloudApi.Messages.Requests;

namespace AutoHelper.Whatsapp.Services;

internal class WhatsappService : IWhatsappService
{
    private readonly IWhatsAppBusinessClient _whatsAppBusinessClient;

    public WhatsappService(IWhatsAppBusinessClient whatsAppBusinessClient)
    {
        _whatsAppBusinessClient = whatsAppBusinessClient;
    }

    public async Task SendConfirmationMessageAsync(string phoneNumber, Guid conversationId, string fromIdentifier)
    {
        var phoneNumberId = GetPhoneNumberId(phoneNumber);
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
                                Text = fromIdentifier
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
        }
        catch (WhatsappBusinessCloudAPIException ex)
        {
            // TODO: Handle with ILogger or on hangfire
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task SendBasicMessageAsync(string phoneNumber, Guid conversationId, string fromIdentifier, string content)
    {
        var phoneNumberId = GetPhoneNumberId(phoneNumber);
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
                                Text = fromIdentifier
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
                                Text = content
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
        }
        catch (WhatsappBusinessCloudAPIException ex)
        {
            // TODO: Handle with ILogger or on hangfire
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task SendVehicleRelatedMessageAsync(string phoneNumber, Guid conversationId, VehicleLookupItem vehicle, string content)
    {
        var phoneNumberId = GetPhoneNumberId(phoneNumber);
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
                                Text = conversationId.ToString().Split('-')[0]// c8e7d5b8
                            },
                            new TextMessageParameter
                            {
                                Type = "text",
                                Text = ""//TODO: DACIA
                            },
                            new TextMessageParameter
                            {
                                Type = "text",
                                Text = ""//TODO: Sandero
                            },
                            new TextMessageParameter
                            {
                                Type = "text",
                                Text = ""//TODO: 2008
                            },
                            new TextMessageParameter
                            {
                                Type = "text",
                                Text = ""//TODO: 0,00KM op 1 liter benzine
                            },
                            new TextMessageParameter
                            {
                                Type = "text",
                                Text = vehicle.MOTExpiryDate.ToShortDateString()// 10-05-2024
                            },
                            new TextMessageParameter
                            {
                                Type = "text",
                                Text = ""//TODO: Logisch
                            },
                            new TextMessageParameter
                            {
                                Type = "text",
                                Text = vehicle.LicensePlate// 87-GRN-6
                            }
                        }
                    },
                }
            }
        };

        try
        {
            var results = await _whatsAppBusinessClient.SendTextMessageTemplateAsync(template);
        }
        catch (WhatsappBusinessCloudAPIException ex)
        {
            // TODO: Handle with ILogger or on hangfire
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    private static string GetPhoneNumberId(string phoneNumber)
    {
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

}