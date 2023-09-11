using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Application.Garages.Models;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Deprecated;
using AutoHelper.Domain.Events;
using AutoMapper;
using MediatR;

namespace AutoHelper.Application.Garages.Commands.UpdateGarageService;


public record UpdateGarageServiceCommand : IRequest<GarageServiceItem>
{
    public Guid Id { get; set; }

    [JsonIgnore]
    public string UserId { get; set; }

    public GarageServiceType Type { get; set; }

    public string Description { get; set; }

    public int DurationInMinutes { get; set; }

    public decimal Price { get; set; }

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
        var entity = await _context.GarageServices.FindAsync(request.Id);
        if (entity == null)
        {
            throw new NotFoundException(nameof(GarageServiceItem), request.Id);
        }

        entity.Type = request.Type;
        entity.Description = request.Description;
        entity.DurationInMinutes = request.DurationInMinutes;
        entity.Price = request.Price;

        // If you wish to use domain events, then you can add them here:
        // entity.AddDomainEvent(new SomeDomainEvent(entity));

        _context.GarageServices.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }
}
