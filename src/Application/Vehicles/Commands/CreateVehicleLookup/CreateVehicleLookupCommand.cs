using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;
using NetTopologySuite.Geometries;

namespace AutoHelper.Application.Vehicles.Commands.CreateVehicleLookup;

public class CreateVehicleLookupCommand : IRequest<VehicleLookupDtoItem>
{
    [Required]
    public string LicensePlate { get; set; } = null!;

    [Required]
    public DateTime MOTExpiryDate { get; set; }

    [Required]
    public string Latitude { get; set; } = null!;

    [Required]
    public string Longitude { get; set; } = null!;

    public string? PhoneNumber { get; set; } = null!;

    public string? WhatsappNumber { get; set; } = null!;

    public string? EmailAddress { get; set; } = null!;

}

public class CreateVehicleLookupCommandHandler : IRequestHandler<CreateVehicleLookupCommand, VehicleLookupDtoItem>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateVehicleLookupCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<VehicleLookupDtoItem> Handle(CreateVehicleLookupCommand request, CancellationToken cancellationToken)
    {
        if (!double.TryParse(request.Latitude, out double latitude) || !double.TryParse(request.Longitude, out double longitude))
        {
            throw new ArgumentException("Invalid latitude or longitude format");
        }

        // SRID 4326 for WGS84 coordinate system
        var location = new Point(longitude, latitude) { SRID = 4326 }; 

        var entity = new VehicleLookupItem()
        {
            LicensePlate = request.LicensePlate,
            MOTExpiryDate = request.MOTExpiryDate,
            Location = location,
            PhoneNumber = request.PhoneNumber,
            WhatsappNumber = request.WhatsappNumber,
            EmailAddress = request.EmailAddress
        };

        // If you wish to use domain events, then you can add them here:
        // entity.AddDomainEvent(new SomeDomainEvent(entity));

        _context.VehicleLookups.Add(entity);
        await _context.SaveChangesAsync(cancellationToken); 

        return _mapper.Map<VehicleLookupDtoItem>(entity);
    }
}
