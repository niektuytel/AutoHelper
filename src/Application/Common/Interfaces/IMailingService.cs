using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Conversations._DTOs;

namespace AutoHelper.Application.Common.Interfaces;

public interface IMailingService
{
    Task SendMessageConfirmation(string receiverContactIdentifier, Guid conversationId, string senderContactName);
    Task SendMessage(string receiverContactIdentifier, Guid conversationId, string senderContactName, string messageContent);
    Task SendMessageWithVehicle(string receiverContactIdentifier, Guid conversationId, VehicleTechnicalDtoItem vehicleInfo, string messageContent);
}
