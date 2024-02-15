using System.Text.RegularExpressions;
using AutoHelper.Application.Common.Extensions;
using AutoHelper.Application.Common.Interfaces;
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

internal class WhatsappService : IWhatsappService
{
    private readonly IConfiguration _configuration;
    
    public WhatsappService(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public string GetPhoneNumberId(string phoneNumber)
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

    /// <summary>
    /// Remove html when message is html
    /// </summary>
    public string GetValidContent(string content)
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