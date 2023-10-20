using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;

namespace AutoHelper.Hangfire.MediatR;

public class MediatorHangfireBridge
{
    private readonly IMediator _mediator;

    public MediatorHangfireBridge(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Send(IBaseRequest command)
    {
        await _mediator.Send(command);
    }

    [DisplayName("{0}")]
    public async Task Send(string jobName, IBaseRequest command)
    {
        await _mediator.Send(command);
    }
}