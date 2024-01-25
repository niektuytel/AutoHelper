using System;
using System.Threading;
using AutoHelper.Hangfire.Shared.Interfaces;
using AutoHelper.Hangfire.Shared.MediatR;
using Hangfire;
using MediatR;

namespace AutoHelper.Hangfire.Shared.MediatR
{
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

        public static string ScheduleJob<T>(this ISender mediator, IBackgroundJobClient client, string queue, string title, IQueueRequest<T> request, DateTimeOffset dateTime)
        {
            queue = queue.ToLower();
            var jobId = client.Schedule<MediatorHangfireBridge>(queue, bridge => bridge.Send(null, queue, title, request, CancellationToken.None), dateTime);
            return jobId;
        }

        public static void StartRecurringJob<T>(this ISender mediator, string jobId, IRequest<T> request, string cron)
        {
            RecurringJob.AddOrUpdate<MediatorHangfireBridge>(jobId, bridge => bridge.Send(request, CancellationToken.None), cron);
        }

        public static void DeleteJob(this ISender mediator, IBackgroundJobClient client, string jobId)
        {
            client.Delete(jobId);
        }
    }
}

