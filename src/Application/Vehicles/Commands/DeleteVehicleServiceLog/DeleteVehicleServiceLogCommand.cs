using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;

namespace AutoHelper.Application.Vehicles.Commands.DeleteVehicleServiceLog;

public class DeleteVehicleServiceLogCommand : IRequest<VehicleServiceLogItem>
{
    public DeleteVehicleServiceLogCommand(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}

public class DeleteVehicleServiceLogCommandHandler : IRequestHandler<DeleteVehicleServiceLogCommand, VehicleServiceLogItem>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public DeleteVehicleServiceLogCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<VehicleServiceLogItem> Handle(DeleteVehicleServiceLogCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.VehicleServiceLogs.FindAsync(request.Id);
        if (entity == null)
        {
            throw new NotFoundException(nameof(VehicleServiceLogItem), request.Id);
        }

        _context.VehicleServiceLogs.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }
}