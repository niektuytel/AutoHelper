namespace AutoHelper.Application.Common.Interfaces;

public interface IQueueService
{
    void DeleteJob(string jobId);
    void Enqueue(string queue, string title, IQueueRequest request, bool isRecursive = false);
    void Enqueue<T>(string queue, string title, IQueueRequest<T> request, bool isRecursive = false);
    string ScheduleJob<T>(string queue, string title, IQueueRequest<T> request, DateTimeOffset dateTime);
    void StartRecurringJob<T>(string jobId, MediatR.IRequest<T> request, string cron);
}