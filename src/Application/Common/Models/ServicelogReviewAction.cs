namespace AutoHelper.Application.Common.Models;

public class ServiceLogReviewAction
{
    public ServiceLogReviewAction(Guid serviceLogId, bool approved)
    {
        ServiceLogId = serviceLogId;
        Approve = approved;
    }

    public Guid ServiceLogId { get; private set; }

    public bool Approve { get; private set; }
}