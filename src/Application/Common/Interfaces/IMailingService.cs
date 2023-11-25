using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Conversations._DTOs;

namespace AutoHelper.Application.Common.Interfaces;

public interface IMailingService
{
    Task SendConfirmationEmailAsync(string receiverContactIdentifier, Guid conversationId, string senderContactName);
    Task SendBasicMailAsync(string receiverContactIdentifier, Guid conversationId, string senderContactName, string messageContent);
    Task SendVehicleRelatedEmailAsync(string receiverContactIdentifier, Guid conversationId, VehicleTechnicalDtoItem vehicleInfo, string messageContent);
}
