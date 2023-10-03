using System.Linq.Expressions;

namespace AutoHelper.Application.Common.Interfaces;

public interface IQueueingJobService
{
    void RunJob<T>(
        string jobId, 
        Expression<Func<T, Task>> methodCall, 
        bool enabled
    );
}