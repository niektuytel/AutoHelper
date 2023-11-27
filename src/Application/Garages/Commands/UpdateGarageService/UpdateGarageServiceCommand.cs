using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Domain.Entities.Garages;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Commands.UpdateGarageService;


public record UpdateGarageServiceCommand : IRequest<GarageServiceItem>
{
    [JsonIgnore]
    public string UserId { get; set; }

    public Guid Id { get; set; }

    public GarageServiceType Type { get; set; }

    public string Description { get; set; }

}

public class UpdateGarageServiceCommandHandler : IRequestHandler<UpdateGarageServiceCommand, GarageServiceItem>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateGarageServiceCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GarageServiceItem> Handle(UpdateGarageServiceCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.GarageServices.FirstOrDefaultAsync(item => item.Id == request.Id && item.UserId == request.UserId, cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException(nameof(GarageServiceItem), request.Id);
        }

        entity.Type = request.Type;
        entity.Description = request.Description;

        // If you wish to use domain events, then you can add them here:
        // entity.AddDomainEvent(new SomeDomainEvent(entity));

        // Since we fetched the entity directly from the DbContext, it's already tracked. 
        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }
}
