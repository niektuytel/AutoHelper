using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;

namespace AutoHelper.Application.Garages._DTOs;

public class GarageServiceDtoItem : IMapFrom<GarageServiceItem>, IMapFrom<GarageLookupServiceItem>
{
    public Guid? Id { get; set; }

    /// <summary>
    /// Like: Inspection, Repair, Maintenance, etc.
    /// </summary>
    public GarageServiceType Type { get; set; } = GarageServiceType.Other;

    /// <summary>
    /// Like: Light Vehicle, Heavy vehicle, etc.
    /// Needed for filtering and made able to make more specific services for each vehicle type
    /// </summary>
    public VehicleType VehicleType { get; set; } = VehicleType.Any;

    /// <summary>
    /// Like: Petrol, Diesel, Electric, etc.
    /// </summary>
    public VehicleFuelType VehicleFuelType { get; set; } = VehicleFuelType.Any;

    /// <summary>
    /// Like: "MOT Service", "Oil Change", "Wheel Alignment", etc.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Like: "Change the oil in the engine", "Align the wheels", etc.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Expected next date
    /// </summary>
    public bool ExpectedNextDateIsRequired { get; set; } = false;

    /// <summary>
    /// Expected next odometer readings
    /// </summary>
    public bool ExpectedNextOdometerReadingIsRequired { get; set; } = false;

    public void Mapping(Profile profile)
    {
        profile.CreateMap<GarageServiceItem, GarageServiceDtoItem>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type))
            .ForMember(d => d.VehicleType, opt => opt.MapFrom(s => s.VehicleType))
            .ForMember(d => d.VehicleFuelType, opt => opt.MapFrom(s => s.VehicleFuelType))
            .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Title))
            .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description))
            .ForMember(d => d.ExpectedNextDateIsRequired, opt => opt.MapFrom(s => s.ExpectedNextDateIsRequired))
            .ForMember(d => d.ExpectedNextOdometerReadingIsRequired, opt => opt.MapFrom(s => s.ExpectedNextOdometerReadingIsRequired))
        ;

        profile.CreateMap<GarageLookupServiceItem, GarageServiceDtoItem>()
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type))
            .ForMember(d => d.VehicleType, opt => opt.MapFrom(s => s.VehicleType))
            .ForMember(d => d.VehicleFuelType, opt => opt.MapFrom(s => s.VehicleFuelType))
            .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Title))
            .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description))
            .ForMember(d => d.ExpectedNextDateIsRequired, opt => opt.MapFrom(s => s.ExpectedNextDateIsRequired))
            .ForMember(d => d.ExpectedNextOdometerReadingIsRequired, opt => opt.MapFrom(s => s.ExpectedNextOdometerReadingIsRequired))
        ;
    }
}
