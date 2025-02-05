﻿using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages._DTOs;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;

namespace AutoHelper.Application.Garages.Commands.CreateGarageServiceItem;

public record CreateGarageServiceCommand : IRequest<GarageServiceDtoItem>
{
    [JsonIgnore]
    public string UserId { get; set; }

    [JsonIgnore]
    public GarageItem? Garage { get; set; } = new GarageItem();

    public GarageServiceType Type { get; set; }

    public VehicleType VehicleType { get; set; }

    public VehicleFuelType VehicleFuelType { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool ExpectedNextDateIsRequired { get; set; } = false;

    public bool ExpectedNextOdometerReadingIsRequired { get; set; } = false;

}

public class CreateGarageServiceItemCommandHandler : IRequestHandler<CreateGarageServiceCommand, GarageServiceDtoItem>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateGarageServiceItemCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<GarageServiceDtoItem> Handle(CreateGarageServiceCommand request, CancellationToken cancellationToken)
    {
        var entity = new GarageServiceItem
        {
            GarageId = request.Garage!.Id,
            Type = request.Type,
            VehicleType = request.VehicleType,
            VehicleFuelType = request.VehicleFuelType,
            Title = request.Title,
            Description = request.Description,
            ExpectedNextDateIsRequired = request.ExpectedNextDateIsRequired,
            ExpectedNextOdometerReadingIsRequired = request.ExpectedNextOdometerReadingIsRequired
        };

        // If you wish to use domain events, then you can add them here:
        // entity.AddDomainEvent(new SomeDomainEvent(entity));

        _context.GarageServices.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<GarageServiceDtoItem>(entity);
    }

}
