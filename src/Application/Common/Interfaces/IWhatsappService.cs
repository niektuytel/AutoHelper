using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoHelper.Application.Common.Interfaces;

public interface IWhatsappService
{
    Task SendConfirmationMessageAsync(string phoneNumber, Guid conversationId, string companyName);
    Task SendBasicMessageAsync(string phoneNumber, Guid conversationId, string fromIdentifier, string content);
}
