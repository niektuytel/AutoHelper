using System.Diagnostics;
using System.Text.Json;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Domain.Entities.Admin;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutoHelper.Application.Common.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly Stopwatch _timer;
    private readonly ILogger<TRequest> _logger;
    private readonly IApplicationDbContext _context;
    private readonly int _performanceThreshold = 2000;  // Can be made configurable

    public PerformanceBehaviour(ILogger<TRequest> logger, IApplicationDbContext context)
    {
        _timer = new Stopwatch();
        _logger = logger;
        _context = context;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;
        if (elapsedMilliseconds > _performanceThreshold)
        {
            await LogPerformanceIssueAsync(request, elapsedMilliseconds, cancellationToken);
        }

        return response;
    }

    private async Task LogPerformanceIssueAsync(TRequest request, long elapsedMilliseconds, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var logMessage = $"Long Running Request: {requestName} ({elapsedMilliseconds} milliseconds)";
        _logger.LogWarning(logMessage);

        var requestLog = new RequestLogItem(request, LogLevel.Warning, logMessage);
        _context.RequestLogs.Add(requestLog);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
