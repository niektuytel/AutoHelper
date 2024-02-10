using AutoHelper.Domain.Entities.Communication;

namespace AutoHelper.Application.Vehicles._DTOs;

public class VehicleNextNotificationItem
{
    public DateTime TriggerDate { get; set; }

    public NotificationVehicleType NotificationType { get; set; }

}