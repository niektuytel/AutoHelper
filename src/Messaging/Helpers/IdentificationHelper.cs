using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using WhatsappBusiness.CloudApi.Interfaces;

namespace AutoHelper.Messaging.Helpers;

public class IdentificationHelper : IIdentificationHelper
{
    private readonly IConfiguration _configuration;
    private readonly bool _isDevelopment;
    private readonly string _developPhoneNumberId;

    public IdentificationHelper(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _isDevelopment = _configuration["Environment"] == "Development";
        _developPhoneNumberId = _configuration["WhatsApp:TestPhoneNumberId"]!;
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

    public string GetValidIdentifier(string? emailAddress, string? whatsappNumber)
    {
        if (string.IsNullOrWhiteSpace(emailAddress) && string.IsNullOrWhiteSpace(whatsappNumber))
        {
            throw new ArgumentException("At least one of the parameters must be filled");
        }

        var identifier = emailAddress;
        if (string.IsNullOrWhiteSpace(identifier))
        {
            identifier = GetPhoneNumberId(whatsappNumber!);
        };

        return identifier;
    }
}
