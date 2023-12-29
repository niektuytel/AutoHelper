using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;
using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Application.Garages._DTOs;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Application.Garages.Queries.GetGarageSettings;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Vehicles.Commands.DeleteVehicleTimeline;

public record DeleteVehicleTimelineCommand : IRequest<VehicleTimelineDtoItem>
{
    public DeleteVehicleTimelineCommand(Guid serviceLogId)
    {
        ServiceLogId = serviceLogId;
    }

    public Guid ServiceLogId { get; set; }

}

public class DeleteVehicleTimelineCommandHandler : IRequestHandler<DeleteVehicleTimelineCommand, VehicleTimelineDtoItem>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public DeleteVehicleTimelineCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<VehicleTimelineDtoItem> Handle(DeleteVehicleTimelineCommand request, CancellationToken cancellationToken)
    {
        var timelineEntity = _context.VehicleTimelineItems.FirstOrDefault(x => x.VehicleServiceLogId == request.ServiceLogId);
        if (timelineEntity != null)
        {
            _context.VehicleTimelineItems.Remove(timelineEntity);
            await _context.SaveChangesAsync(cancellationToken);
            //entity.AddDomainEvent(new SomeDomainEvent(entity));
        }

        return _mapper.Map<VehicleTimelineDtoItem>(timelineEntity);
    }

}
