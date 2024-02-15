using AutoHelper.Application.Messages._DTOs;
using AutoHelper.Domain.Entities.Messages;
using global::Microsoft.AspNetCore.Components;

namespace AutoHelper.Messaging.Templates.Notification;

public partial class VehicleServiceReviewApproved
{
    public static string Subject => "Aanvraag goedgekeurd";

    [Parameter]
    public NotificationItem Notification { get; set; } = new NotificationItem();

    [Parameter]
    public VehicleTechnicalDtoItem VehicleInfo { get; set; } = null!;

    public string DomainUrl { get; set; } = "https://autohelper.nl";

    public string VehicleUrl => $"{DomainUrl}/vehicle/{Notification.VehicleLicensePlate}";

}