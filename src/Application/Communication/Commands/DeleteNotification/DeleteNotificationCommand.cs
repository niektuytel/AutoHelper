using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Interfaces.Queue;
using AutoHelper.Domain.Entities.Messages;
using AutoHelper.WebUI.Controllers;
using AutoMapper;
using MediatR;

namespace AutoHelper.Application.Messages.Commands.DeleteNotification;

public record DeleteNotificationCommand : IRequest<NotificationItemDto>
{
    public DeleteNotificationCommand(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }

    [JsonIgnore]
    public NotificationItem? Notification { get; set; } = null!;

}

public class DeleteNotificationCommandHandler : IRequestHandler<DeleteNotificationCommand, NotificationItemDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IQueueService _queueService;
    private readonly ISender _sender;
    private readonly IMapper _mapper;


    public DeleteNotificationCommandHandler(IApplicationDbContext context, IQueueService queueService, ISender sender, IMapper mapper)
    {
        _context = context;
        _queueService = queueService;
        _sender = sender;
        _mapper = mapper;
    }

    public async Task<NotificationItemDto> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
    {
        // Delete the job if it exists
        if (request.Notification?.JobId != null)
        {
            _queueService.DeleteJob(request.Notification.JobId);
        }

        _context.Notifications.Remove(request.Notification!);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<NotificationItemDto>(request.Notification);
    }
}
