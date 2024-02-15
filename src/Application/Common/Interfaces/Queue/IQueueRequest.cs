using MediatR;

namespace AutoHelper.Application.Common.Interfaces.Queue;

public interface IQueueRequest : IRequest
{
    public IQueueContext QueueService { get; set; }

}


public interface IQueueRequest<T> : IRequest<T>
{
    public IQueueContext QueueingService { get; set; }

}