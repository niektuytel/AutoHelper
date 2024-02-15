using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Models;
using AutoHelper.Application.Messages._DTOs;
using AutoHelper.Domain.Entities.Messages;
using AutoHelper.Domain.Entities.Vehicles;
using global::Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace AutoHelper.Messaging.Templates.Notification;

public partial class VehicleServiceReview
{
    public static string Subject => "Verzoek om Bevestiging van Werkzaamheden";

    [Inject]
    public IAesEncryptionService AesEncryption { get; set; } = null!;

    [Parameter]
    public NotificationItem Notification { get; set; } = null!;

    [Parameter]
    public VehicleTechnicalDtoItem VehicleInfo { get; set; } = null!;

    public string DomainUrl { get; set; } = "https://autohelper.nl";

    public string VehicleUrl => $"{DomainUrl}/vehicle/{Notification.VehicleLicensePlate}";

    public string Description
    {
        get
        {
            var success = Notification.Metadata.TryGetValue("desciption", out var desciption);
            if (!success)
            {
                return string.Empty;
            }

            return desciption!;
        }
    }

    public string ApproveUrl {
        get
        {
            var success = Notification.Metadata.TryGetValue("serviceLogId", out var serviceLogIdString);
            if (!success)
            {
                return string.Empty;
            }
            var serviceLogId = Guid.Parse(serviceLogIdString!);

            var action = new ServiceLogReviewAction(serviceLogId, true);
            var actionJson = JsonConvert.SerializeObject(action);
            var actionEncrypted = AesEncryption.Encrypt(actionJson);
            return $"{DomainUrl}/api/garage/ServiceLogReview?action={actionEncrypted}";
        }
    }

    public string DeclineUrl
    {
        get
        {
            var success = Notification.Metadata.TryGetValue("serviceLogId", out var serviceLogIdString);
            if (!success)
            {
                return string.Empty;
            }
            var serviceLogId = Guid.Parse(serviceLogIdString!);

            var action = new ServiceLogReviewAction(serviceLogId, false);
            var actionJson = JsonConvert.SerializeObject(action);
            var actionEncrypted = AesEncryption.Encrypt(actionJson);
            return $"{DomainUrl}/api/garage/ServiceLogReview?action={actionEncrypted}";
        }
    }
}