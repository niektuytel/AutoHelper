using AutoHelper.Application.Messages._DTOs;
using AutoHelper.Domain.Entities.Messages;
using global::Microsoft.AspNetCore.Components;

namespace AutoHelper.Messaging.Templates.Notification;

public partial class VehicleServiceNotification_MOT
{
    public static string Subject => "APK";

    [Parameter]
    public NotificationItem Notification { get; set; } = new NotificationItem();

    [Parameter]
    public VehicleTechnicalDtoItem VehicleInfo { get; set; } = new VehicleTechnicalDtoItem();

    public string DomainUrl => "https://autohelper.nl";

    public string VehicleUrl => $"{DomainUrl}/vehicle/{Notification.VehicleLicensePlate}";

    public string UnsubscribeUrl => $"{DomainUrl}/api/vehicle/UnsubscribeNotification/{Notification.Id}";
}