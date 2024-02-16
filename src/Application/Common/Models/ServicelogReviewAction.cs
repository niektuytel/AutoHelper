using System.Text.Json.Serialization;

namespace AutoHelper.Application.Common.Models;

public class ServiceLogReviewAction
{
    public ServiceLogReviewAction()
    {
    }

    public ServiceLogReviewAction(Guid serviceLogId, bool approved)
    {
        ServiceLogId = serviceLogId;
        Approve = approved;
    }

    [JsonPropertyName("ServiceLogId")]
    public Guid ServiceLogId { get; set; }

    [JsonPropertyName("Approve")]
    public bool Approve { get; set; }
}