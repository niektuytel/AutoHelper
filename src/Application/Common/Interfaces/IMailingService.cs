using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Vehicles._DTOs;

namespace AutoHelper.Application.Common.Interfaces;

public interface IMailingService
{
    Task SendConfirmationEmailAsync(string receiverContactIdentifier, Guid conversationId, string senderContactName);
    Task SendVehicleRelatedEmailAsync(string receiverContactIdentifier, Guid conversationId, VehicleTechnicalBriefDtoItem vehicleInfo, string subject, string messageContent);
    Task SendBasicMailAsync(string receiverContactIdentifier, Guid conversationId, string senderContactName, string subject, string messageContent);
}
