using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace AutoHelper.Application.Common.Interfaces;

public interface IQueueRequest : IRequest
{
    public IQueueService QueueService { get; set; }
}


public interface IQueueRequest<T> : IRequest<T>
{
    public IQueueService QueueingService { get; set; }
}