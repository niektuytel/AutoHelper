using AutoHelper.Domain.Entities.Messages;
using global::Microsoft.AspNetCore.Components;

namespace AutoHelper.Messaging.Templates.Notification;

public partial class VehicleServiceNotification_SummerService
{
    public static string Subject => "Zomerbeurt";

    [Parameter]
    public NotificationItem Notification { get; set; } = new NotificationItem();

    public string DomainUrl => "https://autohelper.nl";

    public string VehicleUrl => $"{DomainUrl}/vehicle/{Notification.VehicleLicensePlate}";

}