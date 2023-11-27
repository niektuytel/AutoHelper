using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Application.Garages.Commands.CreateGarageServiceItem;
using AutoHelper.Domain.Entities.Garages;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Commands.CreateGarageServiceItem;

public record CreateGarageServiceCommand : IRequest<GarageServiceItem>
{
    [JsonIgnore]
    public string UserId { get; set; }

    [JsonIgnore]
    public GarageItem? Garage { get; set; } = new GarageItem();

    public GarageServiceType Type { get; set; }

    public string Description { get; set; }

}

public class CreateGarageServiceItemCommandHandler : IRequestHandler<CreateGarageServiceCommand, GarageServiceItem>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateGarageServiceItemCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<GarageServiceItem> Handle(CreateGarageServiceCommand request, CancellationToken cancellationToken)
    {
        var entity = new GarageServiceItem
        {
            UserId = request.UserId,
            GarageId = request.Garage!.Id,
            Type = request.Type,
            Description = request.Description
        };

        // If you wish to use domain events, then you can add them here:
        // entity.AddDomainEvent(new SomeDomainEvent(entity));

        _context.GarageServices.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

}
