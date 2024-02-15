using AutoHelper.Application.Common.Interfaces.Queue;
using Hangfire;
using MediatR;

namespace AutoHelper.Hangfire.MediatR;

public static class MediatorExtensions
{
    /// <param name="isRecursive">Enqueue response until repsonse is not MediatR.IBaseRequest</param>
    public static void Enqueue(this ISender mediator, IBackgroundJobClient client, string queue, string title, IQueueRequest request, bool isRecursive = false)
    {
        queue = queue.ToLower();
        if (isRecursive)
        {
            client.Enqueue<MediatorHangfireBridge>(bridge => bridge.SendMany(null, queue, title, request, CancellationToken.None));
        }
        else
        {
            client.Enqueue<MediatorHangfireBridge>(bridge => bridge.Send(null, queue, title, request, CancellationToken.None));
        }
    }

    /// <param name="isRecursive">Enqueue response until repsonse is not MediatR.IBaseRequest</param>
    public static void Enqueue<T>(this ISender mediator, IBackgroundJobClient client, string queue, string title, IQueueRequest<T> request, bool isRecursive = false)
    {
        queue = queue.ToLower();
        if (isRecursive)
        {
            client.Enqueue<MediatorHangfireBridge>(bridge => bridge.SendMany(null, queue, title, request, CancellationToken.None));
        }
        else
        {
            client.Enqueue<MediatorHangfireBridge>(bridge => bridge.Send(null, queue, title, request, CancellationToken.None));
        }
    }

    public static void StartRecurringJob(this ISender mediator, string jobId, IRequest request, string cron)
    {
        RecurringJob.AddOrUpdate<MediatorHangfireBridge>(jobId, bridge => bridge.Send(request, CancellationToken.None), cron);
    }

    public static void RemoveRecurringJob(this ISender mediator, string jobId)
    {
        RecurringJob.RemoveIfExists(jobId);
    }
}