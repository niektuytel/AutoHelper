using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace AutoHelper.Hangfire.Shared.Interfaces
{
    public interface IQueueRequest : IRequest
    {
        public IQueueService QueueService { get; set; }

    }


    public interface IQueueRequest<T> : IRequest<T>
    {
        public IQueueService QueueingService { get; set; }

    }
}