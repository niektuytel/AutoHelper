using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Interfaces;
using Hangfire;
using Hangfire.Server;
using Hangfire.States;
using MediatR;
using static System.Net.Mime.MediaTypeNames;

namespace AutoHelper.Hangfire.MediatR;

public static class MediatorExtensions
{
    /// <param name="isRecursive">Enqueue response until repsonse is not MediatR.IBaseRequest</param>
    public static void Enqueue(this ISender mediator, string queue, string title, IQueueRequest request, bool isRecursive = false)
    {
        var client = new BackgroundJobClient();

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
    public static void Enqueue<T>(this ISender mediator, string queue, string title, IQueueRequest<T> request, bool isRecursive = false)
    {
        var client = new BackgroundJobClient();

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

    public static void RecurringJobWeekly(this IMediator mediator, string jobId, IRequest request, bool enabled)
    {
        if (enabled)
        {
            RecurringJob.AddOrUpdate<MediatorHangfireBridge>(jobId, bridge => bridge.Send(request, CancellationToken.None), Cron.Weekly);
        }
        else
        {
            Console.Write($"Recurring jobs [{jobId}] are disabled.");
            RecurringJob.RemoveIfExists(jobId);
        }
    }
}