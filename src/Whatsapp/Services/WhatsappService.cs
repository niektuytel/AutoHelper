using System.Net.Http;
using AutoHelper.Application.Common.Extensions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain.Entities.Vehicles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WhatsappBusiness.CloudApi;
using WhatsappBusiness.CloudApi.Exceptions;
using WhatsappBusiness.CloudApi.Interfaces;
using WhatsappBusiness.CloudApi.Messages.Requests;

namespace AutoHelper.Whatsapp.Services;

internal class WhatsappService : IWhatsappService
{
    private readonly IWhatsAppBusinessClient _whatsAppBusinessClient;
    private readonly IConfiguration _configuration;
    private readonly bool _isDevelopment;
    private readonly string _developPhoneNumberId;

    public WhatsappService(IWhatsAppBusinessClient whatsAppBusinessClient, IConfiguration configuration)
    {
        _whatsAppBusinessClient = whatsAppBusinessClient ?? throw new ArgumentNullException(nameof(whatsAppBusinessClient));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _isDevelopment = _configuration["Environment"] == "Development";
        _developPhoneNumberId = _configuration["WhatsApp:TestPhoneNumberId"]!;
    }

    public async Task SendConfirmationMessageAsync(string phoneNumber, Guid conversationId, string fromContactName)
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
        }
        catch (WhatsappBusinessCloudAPIException ex)
        {
            // TODO: Handle with ILogger or on hangfire
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task SendBasicMessageAsync(string phoneNumber, Guid conversationId, string fromContactName, string content)
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

    public async Task SendVehicleRelatedMessageAsync(string phoneNumber, Guid conversationId, VehicleTechnicalBriefDtoItem vehicle, string content)
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
                                Text = vehicle.LicensePlate// 87-GRN-6
                            },
                            new TextMessageParameter
                            {
                                Type = "text",
                                Text = vehicle.FuelType.ToCamelCase()// Benzine
                            },
                            new TextMessageParameter
                            {
                                Type = "text",
                                Text = $"{vehicle.Brand.ToCamelCase()} {vehicle.Model.ToCamelCase()}({vehicle.YearOfFirstAdmission})"// Dacia Sandero (2008)
                            },
                            new TextMessageParameter
                            {
                                Type = "text",
                                Text = vehicle.MOTExpiryDate// 10-05-2024
                            },
                            new TextMessageParameter
                            {
                                Type = "text",
                                Text = vehicle.Mileage.ToCamelCase()// Logisch
                            },
                            new TextMessageParameter
                            {
                                Type = "text",
                                Text = conversationId.ToString().Split('-')[0]// c8e7d5b8
                            },
                        }
                    },

                    // TODO: Add "Meer info:   https://"
                    // TODO: Remove "Antwoorden kan door middel van antwoord geven op dit bericht."
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

    private string GetPhoneNumberId(string phoneNumber)
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

}