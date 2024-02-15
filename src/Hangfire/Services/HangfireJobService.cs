using AutoHelper.Application.Common.Interfaces.Queue;
using AutoHelper.Hangfire.Extentions;
using Hangfire;
using MediatR;

namespace AutoHelper.Hangfire.Services;

internal class HangfireJobService : IQueueService
{
    private readonly IBackgroundJobClient _client;

    public HangfireJobService(IBackgroundJobClient client)
    {
        _client = client;
    }

    /// <param name="isRecursive">Enqueue response until repsonse is not MediatR.IBaseRequest</param>
    public void Enqueue(string queue, string title, IQueueRequest request, bool isRecursive = false)
    {
        queue = queue.ToLower();
        if (isRecursive)
        {
            _client.Enqueue<MediatorHangfireBridge>(bridge => bridge.SendMany(null, queue, title, request, CancellationToken.None));
        }
        else
        {
            _client.Enqueue<MediatorHangfireBridge>(bridge => bridge.Send(null, queue, title, request, CancellationToken.None));
        }
    }

    /// <param name="isRecursive">Enqueue response until repsonse is not MediatR.IBaseRequest</param>
    public void Enqueue<T>(string queue, string title, IQueueRequest<T> request, bool isRecursive = false)
    {
        queue = queue.ToLower();
        if (isRecursive)
        {
            _client.Enqueue<MediatorHangfireBridge>(bridge => bridge.SendMany(null, queue, title, request, CancellationToken.None));

        }
        else
        {
            _client.Enqueue<MediatorHangfireBridge>(bridge => bridge.Send(null, queue, title, request, CancellationToken.None));
        }
    }

    public string ScheduleJob<T>(string queue, string title, IQueueRequest<T> request, DateTimeOffset dateTime)
    {
#if DEBUG
        dateTime = DateTime.Now.AddSeconds(10);
#endif

        queue = queue.ToLower();
        var jobId = _client.Schedule<MediatorHangfireBridge>(queue, bridge => bridge.Send(null, queue, title, request, CancellationToken.None), dateTime);
        return jobId;
    }

    public void StartRecurringJob<T>(string jobId, IRequest<T> request, string cron)
    {
        RecurringJob.AddOrUpdate<MediatorHangfireBridge>(jobId, bridge => bridge.Send(request, CancellationToken.None), cron);
    }

    public void DeleteJob(string jobId)
    {
        _client.Delete(jobId);
    }
}
