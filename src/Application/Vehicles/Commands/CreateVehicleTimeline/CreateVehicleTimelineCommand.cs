using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;

namespace AutoHelper.Application.Vehicles.Commands.CreateVehicleTimeline;

public record CreateVehicleTimelineCommand : IRequest<VehicleTimelineDtoItem>
{
    public CreateVehicleTimelineCommand(VehicleServiceLogItem serviceLog)
    {
        ServiceLog = serviceLog;
    }

    public VehicleServiceLogItem ServiceLog { get; set; }
}

public class CreateVehicleServiceLogCommandHandler : IRequestHandler<CreateVehicleTimelineCommand, VehicleTimelineDtoItem>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IVehicleTimelineService _vehicleTimelineService;

    public CreateVehicleServiceLogCommandHandler(IApplicationDbContext context, IMapper mapper, IVehicleTimelineService vehicleTimelineService)
    {
        _context = context;
        _mapper = mapper;
        _vehicleTimelineService = vehicleTimelineService;
    }

    public async Task<VehicleTimelineDtoItem> Handle(CreateVehicleTimelineCommand request, CancellationToken cancellationToken)
    {
        var entity = _vehicleTimelineService.CreateServiceLogItem(request.ServiceLog);

        _context.VehicleTimelineItems.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        //entity.AddDomainEvent(new SomeDomainEvent(entity));

        return _mapper.Map<VehicleTimelineDtoItem>(entity);
    }

}
