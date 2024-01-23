using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Messages._DTOs;

namespace AutoHelper.Application.Common.Interfaces;

public interface IMailingService : IMessagingService
{
    Task SendMessageRaw(string receiverIdentifier, Guid conversationId, string senderName, string message);
}
