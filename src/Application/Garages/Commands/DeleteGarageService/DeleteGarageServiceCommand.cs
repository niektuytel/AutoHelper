using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages._DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Commands.DeleteGarageService;


public record DeleteGarageServiceCommand : IRequest<GarageServiceDtoItem>
{

    public DeleteGarageServiceCommand(Guid id, string userId)
    {
        Id = id;
        UserId = userId;
    }
    public Guid Id { get; set; }

    [JsonIgnore]
    public string? UserId { get; set; }
}

public class DeleteGarageServiceCommandHandler : IRequestHandler<DeleteGarageServiceCommand, GarageServiceDtoItem>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public DeleteGarageServiceCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GarageServiceDtoItem> Handle(DeleteGarageServiceCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.GarageServices
            .Include(item => item.Garage)
            .FirstOrDefaultAsync(item => item.Id == request.Id && item.Garage.UserId == request.UserId, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(GarageServiceDtoItem), request.Id);
        }

        // If you wish to use domain events, then you can add them here:
        // entity.AddDomainEvent(new SomeDomainEvent(entity));

        _context.GarageServices.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<GarageServiceDtoItem>(entity);
    }
}
