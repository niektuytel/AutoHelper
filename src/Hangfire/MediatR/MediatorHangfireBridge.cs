using System.ComponentModel;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Exceptions;
using Hangfire;
using MediatR;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;

namespace AutoHelper.Hangfire.MediatR;

public class MediatorHangfireBridge
{
    private readonly IMediator _mediator;

    public MediatorHangfireBridge(IMediator mediator)
    {
        _mediator = mediator;
    }


    [Queue("{0}")]
    [DisplayName("{0}")]
    public async Task Send(string queue, IBaseRequest command)
    {
        await Send(command);
    }

    public async Task Send(IBaseRequest command)
    {
        await _mediator.Send(command);
    }

    public async Task SendMany(object? request)
    {
        var nextStep = true;

        while (nextStep)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            try
            {
                request = await _mediator.Send(request);
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