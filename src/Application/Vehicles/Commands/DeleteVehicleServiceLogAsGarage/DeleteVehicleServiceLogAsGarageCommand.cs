using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Messages.Commands.CreateNotificationMessage;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Commands.DeleteVehicleTimeline;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Messages.Enums;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;

namespace AutoHelper.Application.Vehicles.Commands.DeleteVehicleServiceLogAsGarage;

public class DeleteVehicleServiceLogAsGarageCommand : IRequest<VehicleServiceLogAsGarageDtoItem>
{
    public DeleteVehicleServiceLogAsGarageCommand(string userId, Guid serviceLogId)
    {
        UserId = userId;
        ServiceLogId = serviceLogId;
    }

    [JsonIgnore]
    public string UserId { get; set; } = null!;

    [JsonIgnore]
    public GarageItem Garage { get; set; } = null!;

    public Guid ServiceLogId { get; set; }

    [JsonIgnore]
    public VehicleServiceLogItem ServiceLog { get; set; } = null!;

}

public class DeleteVehicleServiceLogAsGarageCommandHandler : IRequestHandler<DeleteVehicleServiceLogAsGarageCommand, VehicleServiceLogAsGarageDtoItem>
{
    private readonly IVehicleService _vehicleService;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ISender _mediator;

    public DeleteVehicleServiceLogAsGarageCommandHandler(IVehicleService vehicleService, IApplicationDbContext context, IMapper mapper, ISender mediator)
    {
        _vehicleService = vehicleService;
        _context = context;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<VehicleServiceLogAsGarageDtoItem> Handle(DeleteVehicleServiceLogAsGarageCommand request, CancellationToken cancellationToken)
    {
        var timelineItem = await _mediator.Send(new DeleteVehicleTimelineCommand(request.ServiceLogId), cancellationToken);

        _context.VehicleServiceLogs.Remove(request.ServiceLog);
        await _context.SaveChangesAsync(cancellationToken);
        //entity.AddDomainEvent(new SomeDomainEvent(entity));

        // Send email to user to notify that the service log is canceled/deleted
        try
        {
            var command = new CreateNotificationCommand(
                request.ServiceLog.VehicleLicensePlate,
                NotificationType.UserServiceReviewDeclined,
                entity.ReporterEmailAddress,
                entity.ReporterPhoneNumber
            );

            var notification = await _sender.Send(command, cancellationToken);
        }
        catch (Exception)
        {
            // TODO: Admin should fix this exception
            throw;
        }

        return _mapper.Map<VehicleServiceLogAsGarageDtoItem>(request.ServiceLog);
    }
}