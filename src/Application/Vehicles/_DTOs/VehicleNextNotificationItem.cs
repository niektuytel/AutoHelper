using AutoHelper.Domain.Entities.Messages.Enums;

namespace AutoHelper.Application.Vehicles._DTOs;

public class VehicleNextNotificationItem
{
    public DateTime TriggerDate { get; set; }

    public VehicleNotificationType NotificationType { get; set; }

}