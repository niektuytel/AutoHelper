using System.Linq.Expressions;
using Hangfire.Server;

namespace AutoHelper.Application.Common.Interfaces;

public interface IQueueService
{
    void Initialize(PerformContext context);
    void LogInformation(string value);
    void LogWarning(string value);
    void LogError(string value);
    void LogProgress(int procentage, bool useNewInstance = false);
}