using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Interfaces.Queue;
using AutoHelper.Application.Common.Models;
using AutoHelper.Application.Messages.Commands.CreateNotificationMessage;
using AutoHelper.Application.Messages.Commands.SendNotificationMessage;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Commands.CreateVehicleTimeline;
using AutoHelper.Domain;
using AutoHelper.Domain.Entities.Communication;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Vehicles.Commands.ReviewVehicleServiceLog;

public record ReviewVehicleServiceLogCommand : IRequest<VehicleServiceLogDtoItem>
{
    public ReviewVehicleServiceLogCommand(string actionString)
    {
        ActionString = actionString;
    }

    internal string ActionString { get; private set; }
    internal bool Approved { get; set; } = false;
    internal VehicleServiceLogItem ServiceLog { get; set; } = null!;
}

public class UpdateVehicleServiceLogAsGarageCommandHandler : IRequestHandler<ReviewVehicleServiceLogCommand, VehicleServiceLogDtoItem>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateVehicleServiceLogAsGarageCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<VehicleServiceLogDtoItem> Handle(ReviewVehicleServiceLogCommand request, CancellationToken cancellationToken)
    {
        if(request.Approved)
        {
            request.ServiceLog.Status = VehicleServiceLogStatus.VerifiedByGarage;
        }
        else
        {
            _context.VehicleServiceLogs.Remove(request.ServiceLog);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return _mapper.Map<VehicleServiceLogDtoItem>(request.ServiceLog);
    }

}
