using MediatR;

namespace AutoHelper.Application.Common.Interfaces;

public interface IQueueRequest : IRequest
{
    public IQueueContext QueueService { get; set; }

}


public interface IQueueRequest<T> : IRequest<T>
{
    public IQueueContext QueueingService { get; set; }

}