using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System.Linq.Expressions;
using AutoHelper.Application.Common.Interfaces;

namespace AutoHelper.Hangfire.Services;

internal class HangfireJobService : IQueueingJobService
{
    private readonly ILogger<HangfireJobService> _logger;

    public HangfireJobService(
        IConfiguration configuration,
        ILogger<HangfireJobService> logger
    )
    {
        _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
    }

    public void RunJob<T>(
        string jobId, 
        Expression<Func<T, Task>> methodCall, 
        bool enabled
    )
    {
        throw new NotImplementedException();
    }

}
