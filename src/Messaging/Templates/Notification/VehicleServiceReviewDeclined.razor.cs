using AutoHelper.Application.Messages._DTOs;
using AutoHelper.Domain.Entities.Messages;
using global::Microsoft.AspNetCore.Components;

namespace AutoHelper.Messaging.Templates.Notification;

public partial class VehicleServiceReviewDeclined
{
    public static string Subject => "Aanvraag afgekeurd";

    [Parameter]
    public NotificationItem Notification { get; set; } = new NotificationItem();

    [Parameter]
    public VehicleTechnicalDtoItem VehicleInfo { get; set; } = null!;

    public string DomainUrl => $"https://autohelper.nl";// https://localhost:5001

    public string VehicleUrl => $"{DomainUrl}/vehicle/{Notification.VehicleLicensePlate}";

}