
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;
using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Application.Messages.Commands.CreateNotificationMessage;
using AutoHelper.Application.Garages._DTOs;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Application.Garages.Queries.GetGarageSettings;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Commands.CreateVehicleServiceLogAsGarage;
using AutoHelper.Domain;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Messages.Enums;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoHelper.Application.Messages.Commands.ScheduleNotification;
using AutoHelper.WebUI.Controllers;
using AutoHelper.Domain.Entities.Messages;

namespace AutoHelper.Application.Vehicles.Commands.DeleteVehicleEventNotifier;

public record DeleteVehicleEventNotifierCommand : IRequest<NotificationItemDto>
{
    public DeleteVehicleEventNotifierCommand(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }

    [JsonIgnore]
    public NotificationItem? Notification { get; set; } = null!;

}

public class DeleteVehicleEventNotifierCommandHandler : IRequestHandler<DeleteVehicleEventNotifierCommand, NotificationItemDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public DeleteVehicleEventNotifierCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<NotificationItemDto> Handle(DeleteVehicleEventNotifierCommand request, CancellationToken cancellationToken)
    {
        var result = _context.Notifications.Remove(request.Notification!);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<NotificationItemDto>(result);
    }
}
