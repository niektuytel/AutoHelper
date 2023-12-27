using System.Diagnostics;
using AutoHelper.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutoHelper.Application.Common.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly Stopwatch _timer;
    private readonly ILogger<TRequest> _logger;

    public PerformanceBehaviour(
        ILogger<TRequest> logger)
    {
        _timer = new Stopwatch();
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogWarning("AutoHelper Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@Request}", requestName, elapsedMilliseconds, request);
        }

        return response;
    }
}
