using global::System;
using global::System.Collections.Generic;
using global::System.Linq;
using global::System.Threading.Tasks;
using global::Microsoft.AspNetCore.Components;
using AutoHelper.Domain.Entities.Messages;

namespace AutoHelper.Messaging.Templates.Notification;

public partial class GarageServiceReviewReminder
{
    [Parameter]
    public NotificationItem Notification { get; set; } = new NotificationItem();

    public string VehicleUrl => $"https://autohelper.nl/select-vehicle/{Notification.VehicleLicensePlate}";

}