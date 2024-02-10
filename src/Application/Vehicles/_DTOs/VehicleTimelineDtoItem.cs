using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;

namespace AutoHelper.Application.Vehicles._DTOs;

public class VehicleTimelineDtoItem : IMapFrom<VehicleTimelineItem>
{
    public string Title { get; set; } = null!;

    public string Description { get; set; }

    public DateTime Date { get; set; }

    public VehicleTimelineType Type { get; set; }

    public List<Tuple<string, string>> ExtraData { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<VehicleTimelineItem, VehicleTimelineDtoItem>()
            .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Title))
            .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description))
            .ForMember(d => d.Date, opt => opt.MapFrom(s => s.Date))
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type))
            .ForMember(d => d.ExtraData, opt => opt.MapFrom(s => s.ExtraData));
    }
}