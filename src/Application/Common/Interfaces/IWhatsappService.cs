using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Conversations._DTOs;

namespace AutoHelper.Application.Common.Interfaces;

public interface IWhatsappService
{
    Task SendConfirmationMessageAsync(string phoneNumber, Guid conversationId, string fromContactName);
    Task SendBasicMessageAsync(string phoneNumber, Guid conversationId, string fromContactName, string content);
    Task SendVehicleRelatedMessageAsync(string phoneNumber, Guid conversationId, VehicleTechnicalDtoItem vehicle, string content);
}
