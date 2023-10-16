using System.Linq.Expressions;
using Hangfire.Server;

namespace AutoHelper.Application.Common.Interfaces;

public interface IQueueingService
{
    void LogInformation(string value);
}