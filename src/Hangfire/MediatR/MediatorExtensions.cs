using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Server;
using Hangfire.States;
using MediatR;
using static System.Net.Mime.MediaTypeNames;

namespace AutoHelper.Hangfire.MediatR;

public static class MediatorExtensions
{
    /// <param name="isRecursive">Enqueue response until repsonse is not MediatR.IBaseRequest</param>
    public static void Enqueue(this ISender mediator, string queue, IBaseRequest request, bool isRecursive = false)
    {
        var client = new BackgroundJobClient();
        if (isRecursive)
        {
            client.Enqueue<MediatorHangfireBridge>(bridge => bridge.SendMany(request));
        }
        else
        {
            if(string.IsNullOrEmpty(queue))
            {
                client.Enqueue<MediatorHangfireBridge>(bridge => bridge.Send(request));
            }
            else
            {
                queue = queue.ToLower();
                client.Enqueue<MediatorHangfireBridge>(bridge => bridge.Send(queue, request));
            }
        }
    }

    public static void RecurringJobWeekly(this IMediator mediator, string jobId, IRequest request, bool enabled)
    {
        if (enabled)
        {
            RecurringJob.AddOrUpdate<MediatorHangfireBridge>(jobId, bridge => bridge.Send(request), Cron.Weekly);
        }
        else
        {
            Console.Write($"Recurring jobs [{jobId}] are disabled.");
            RecurringJob.RemoveIfExists(jobId);
        }
    }
}