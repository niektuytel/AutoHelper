using System.ComponentModel;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces.Queue;
using Hangfire;
using Hangfire.Server;
using MediatR;
using Newtonsoft.Json;

namespace AutoHelper.Hangfire.MediatR;

public class MediatorHangfireBridge
{
    private readonly IMediator _mediator;
    private readonly IQueueContext _queueContext;

    public MediatorHangfireBridge(IMediator mediator, IQueueContext queueContext)
    {
        _mediator = mediator;
        _queueContext = queueContext;
    }

    [Queue("{1}")]
    [DisplayName("{2}")]
    public async Task Send(PerformContext context, string queue, string displayName, IQueueRequest command, CancellationToken cancellationToken)
    {
        _queueContext.Initialize(context);
        command.QueueService = _queueContext;

        await _mediator.Send(command, cancellationToken);
    }

    [Queue("{1}")]
    [DisplayName("{2}")]
    public async Task Send<T>(PerformContext context, string queue, string displayName, IQueueRequest<T> command, CancellationToken cancellationToken)
    {
        _queueContext.Initialize(context);
        command.QueueingService = _queueContext;

        await _mediator.Send(command, cancellationToken);
    }

    public async Task Send(IBaseRequest command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
    }

    [Queue("{1}")]
    [DisplayName("{2}")]
    public async Task SendMany(PerformContext context, string queue, string displayName, object? request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var nextStep = true;
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        _queueContext.Initialize(context);
        (request as IQueueRequest)!.QueueService = _queueContext;

        while (nextStep)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            try
            {
                request = await _mediator.Send(request, cancellationToken);
                nextStep = request != null && request is IBaseRequest;
            }
            catch (ValidationException ex)
            {
                var errors = JsonConvert.SerializeObject(ex.Errors);
                throw new Exception($"message: {ex.Message}\nerrors:\n\n{errors}");
            }
        }
    }

}