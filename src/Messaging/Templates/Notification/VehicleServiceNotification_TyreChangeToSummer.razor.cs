using AutoHelper.Domain.Entities.Messages;
using global::Microsoft.AspNetCore.Components;

namespace AutoHelper.Messaging.Templates.Notification;

public partial class VehicleServiceNotification_TyreChangeToSummer
{
    public static string Subject => "Zomerbandenwissel";

    [Parameter]
    public string DomainUrl { get; set; } = "https://autohelper.nl";

    [Parameter]
    public NotificationItem Notification { get; set; } = new NotificationItem();

    public string VehicleUrl => $"{DomainUrl}/vehicle/{Notification.VehicleLicensePlate}";

    public string UnsubscribeUrl => $"{DomainUrl}/api/vehicle/UnsubscribeNotification/{Notification.Id}";
}