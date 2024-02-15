using AutoHelper.Application.Messages._DTOs;
using AutoHelper.Domain.Entities.Messages;

namespace AutoHelper.Application.Common.Interfaces.Messaging;

public interface INotificationService
{
    Task SendNotification(NotificationItem notification, VehicleTechnicalDtoItem vehicle, CancellationToken cancellationToken);
}
