using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Server;
using MediatR;

namespace AutoHelper.Hangfire.MediatR;

public static class MediatorExtensions
{
    public static void Enqueue(this ISender mediator, string jobName, IRequest request)
    {
        var client = new BackgroundJobClient();
        client.Enqueue<MediatorHangfireBridge>(bridge => bridge.Send(jobName, request));
    }

    public static void Enqueue(this IMediator mediator, string jobName, IRequest request)
    {
        var client = new BackgroundJobClient();
        client.Enqueue<MediatorHangfireBridge>(bridge => bridge.Send(jobName, request));
    }

    public static void Enqueue(this IMediator mediator, IRequest request)
    {
        var client = new BackgroundJobClient();
        client.Enqueue<MediatorHangfireBridge>((bridge) => bridge.Send(request));
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