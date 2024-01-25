using global::System;
using global::System.Collections.Generic;
using global::System.Linq;
using global::System.Threading.Tasks;
using global::Microsoft.AspNetCore.Components;

namespace AutoHelper.Messaging.Templates.Conversation;

public partial class MessageWithVehicle
{
    [Parameter]
    public string LicensePlate { get; set; } = string.Empty;

    [Parameter]
    public string Model { get; set; } = string.Empty;

    [Parameter]
    public string Content { get; set; } = string.Empty;

    [Parameter]
    public string ConversationId { get; set; } = string.Empty;

    [Parameter]
    public string FuelType { get; set; } = string.Empty;

    [Parameter]
    public string MOT { get; set; } = string.Empty;

    [Parameter]
    public string NAP { get; set; } = string.Empty;

    public string VehicleUrl => $"https://autohelper.nl/select-vehicle/{LicensePlate}";
}