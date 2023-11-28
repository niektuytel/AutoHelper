using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AutoHelper.Domain.Entities.Vehicles;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Application.Common.Mappings;
using AutoMapper;

namespace AutoHelper.Application.Garages._DTOs;

public class GarageServiceDtoItem : IMapFrom<GarageServiceItem>
{
    public Guid Id { get; set; }

    public GarageServiceType Type { get; set; } = GarageServiceType.Other;

    public string Description { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<GarageServiceItem, GarageServiceDtoItem>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type))
            .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description));
    }
}
