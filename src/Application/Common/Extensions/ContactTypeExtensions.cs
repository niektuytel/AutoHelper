using System.Text.RegularExpressions;
using AutoHelper.Domain.Common.Enums;

namespace AutoHelper.Application.Common.Extensions;

internal static class ContactTypeExtensions
{
    private const string EmailPattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";
    private const string WhatsappPattern = @"^(\+?[0-9]{1,2}[0-9]{3,14}|0[0-9]{8,9})$";

    internal static ContactType GetContactType(this string senderWhatsAppNumberOrEmail)
    {
        if (Regex.IsMatch(senderWhatsAppNumberOrEmail, EmailPattern))
        {
            return ContactType.Email;
        }
        else if (Regex.IsMatch(senderWhatsAppNumberOrEmail, WhatsappPattern))
        {
            return ContactType.WhatsApp;
        }
        else
        {
            throw new ArgumentException("Invalid sender contact type");
        }
    }
}
