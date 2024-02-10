using AutoHelper.Domain.Entities.Messages;
using global::Microsoft.AspNetCore.Components;

namespace AutoHelper.Messaging.Templates.Notification;

public partial class VehicleServiceNotification_SummerTyreChange
{
    [Parameter]
    public NotificationItem Notification { get; set; } = new NotificationItem();

    public string VehicleUrl => $"https://autohelper.nl/vehicle/{Notification.VehicleLicensePlate}";

}