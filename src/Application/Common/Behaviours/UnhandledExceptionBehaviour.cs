using System.Text.Json;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Domain.Entities.Admin;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutoHelper.Application.Common.Behaviours;

public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TRequest> _logger;
    private readonly IApplicationDbContext _context;

    public UnhandledExceptionBehaviour(ILogger<TRequest> logger, IApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        when (ex is ForbiddenAccessException || ex is NotFoundException || ex is ValidationException)
        {
            var message = ex.Message;
            if (ex is ValidationException validationEx)
            {
                message = JsonSerializer.Serialize(validationEx.Errors);
            }

            await LogExceptionAsync(request, LogLevel.Warning, message, cancellationToken);
            throw;
        }
        catch (Exception ex)
        {
            await LogExceptionAsync(request, LogLevel.Critical, ex.Message, cancellationToken);
            throw;
        }
    }

    private async Task LogExceptionAsync(TRequest request, LogLevel logLevel, string message, CancellationToken cancellationToken)
    {
        var requestLog = new RequestLogItem(request, logLevel, message);
        _context.RequestLogs.Add(requestLog);
        await _context.SaveChangesAsync(cancellationToken);

        var requestName = typeof(TRequest).Name;
        _logger.Log(logLevel, $"Request: {logLevel} for Request {requestName}: {message}");
    }
}
