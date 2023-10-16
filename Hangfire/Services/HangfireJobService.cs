using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System.Linq.Expressions;
using AutoHelper.Application.Common.Interfaces;
using Hangfire.Server;

namespace AutoHelper.Hangfire.Services;

internal class HangfireJobService : IQueueingService
{
    public HangfireJobService()
    {
    }

    public void LogInformation(string value)
    {
        // TODO: set logging on Hangfire dashboard
        Console.WriteLine(value);
    }
}
