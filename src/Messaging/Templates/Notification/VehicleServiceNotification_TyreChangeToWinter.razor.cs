using AutoHelper.Domain.Entities.Messages;
using global::Microsoft.AspNetCore.Components;

namespace AutoHelper.Messaging.Templates.Notification;

public partial class VehicleServiceNotification_TyreChangeToWinter
{
    public static string Subject => "Winterbandenwissel";

    [Parameter]
    public NotificationItem Notification { get; set; } = new NotificationItem();

    public string DomainUrl => "https://autohelper.nl";

    public string VehicleUrl => $"{DomainUrl}/vehicle/{Notification.VehicleLicensePlate}";

}