using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages._DTOs;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Commands.UpdateGarageService;


public record UpdateGarageServiceCommand : IRequest<GarageServiceDtoItem>
{
    [JsonIgnore]
    public string UserId { get; set; }

    public Guid Id { get; set; }

    public GarageServiceType Type { get; set; }

    public VehicleType VehicleType { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool ExpectedNextDateIsRequired { get; set; } = false;

    public bool ExpectedNextOdometerReadingIsRequired { get; set; } = false;

}

public class UpdateGarageServiceCommandHandler : IRequestHandler<UpdateGarageServiceCommand, GarageServiceDtoItem>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateGarageServiceCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GarageServiceDtoItem> Handle(UpdateGarageServiceCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.GarageServices
            .Include(item => item.Garage)
            .FirstOrDefaultAsync(item => item.Id == request.Id && item.Garage.UserId == request.UserId, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(GarageServiceItem), request.Id);
        }

        entity.Type = request.Type;
        entity.VehicleType = request.VehicleType;
        entity.Title = request.Title;
        entity.Description = request.Description;
        entity.ExpectedNextDateIsRequired = request.ExpectedNextDateIsRequired;
        entity.ExpectedNextOdometerReadingIsRequired = request.ExpectedNextOdometerReadingIsRequired;

        // If you wish to use domain events, then you can add them here:
        // entity.AddDomainEvent(new SomeDomainEvent(entity));

        // Since we fetched the entity directly from the DbContext, it's already tracked. 
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<GarageServiceDtoItem>(entity);
    }
}
