using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using AutoHelper.Hangfire.Shared.Interfaces;
using FluentValidation;
using Hangfire;
using Hangfire.Server;
using MediatR;
using Newtonsoft.Json;

namespace AutoHelper.Hangfire.Shared.MediatR
{
    public class MediatorHangfireBridge
    {
        private readonly IMediator _mediator;
        private readonly IQueueService _queueJobService;

        public MediatorHangfireBridge(IMediator mediator, IQueueService queueJobService)
        {
            _mediator = mediator;
            _queueJobService = queueJobService;
        }

        [Queue("{1}")]
        [DisplayName("{2}")]
        [AutomaticRetry(Attempts = 1, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async Task Send(PerformContext context, string queue, string displayName, IQueueRequest command, CancellationToken cancellationToken)
        {
            _queueJobService.Initialize(context);
            command.QueueService = _queueJobService;

            await _mediator.Send(command, cancellationToken);
        }

        [Queue("{1}")]
        [DisplayName("{2}")]
        [AutomaticRetry(Attempts = 1, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async Task Send<T>(PerformContext context, string queue, string displayName, IQueueRequest<T> command, CancellationToken cancellationToken)
        {
            _queueJobService.Initialize(context);
            command.QueueingService = _queueJobService;

            await _mediator.Send(command, cancellationToken);
        }

        public async Task Send(IBaseRequest command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);
        }

        [Queue("{1}")]
        [DisplayName("{2}")]
        [AutomaticRetry(Attempts = 1, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async Task SendMany(PerformContext context, string queue, string displayName, object? request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var nextStep = true;
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            _queueJobService.Initialize(context);
            (request as IQueueRequest)!.QueueService = _queueJobService;

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
}