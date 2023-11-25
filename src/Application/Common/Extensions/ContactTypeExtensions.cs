using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoHelper.Domain.Entities.Conversations.Enums;

namespace AutoHelper.Application.Common.Extensions;

internal static class ContactTypeExtensions
{
    private const string EmailPattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";
    private const string WhatsappPattern = @"^(\+?[1-9]{1}[0-9]{3,14}|[0-9]{9,10})$";

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
