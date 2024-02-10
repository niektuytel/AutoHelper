using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain.Entities.Garages;

namespace AutoHelper.Application.Garages._DTOs;

public class GarageSettingsDtoItem : IMapFrom<GarageItem>
{
    public string Name { get; set; }

    public string Address { get; set; }

    public string City { get; set; }

    public string? Image { get; set; } = null;
    public string? ImageThumbnail { get; set; } = null;

    public string? PhoneNumber { get; set; }

    public string? WhatsappNumber { get; set; }

    public string? EmailAddress { get; set; }

    /// <summary>
    /// Contact identifier for the conversation with the garage.
    /// This can be a email address or whatsapp number.
    /// When defined, the garage is available for conversation.
    /// </summary>
    public string? ConversationContactEmail { get; set; }
    public string? ConversationContactWhatsappNumber { get; set; }

    public string? Website { get; set; }

    public void Mapping(AutoMapper.Profile profile)
    {
        profile.CreateMap<GarageItem, GarageSettingsDtoItem>()
            .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Lookup.Name))
            .ForMember(d => d.Address, opt => opt.MapFrom(s => s.Lookup.Address))
            .ForMember(d => d.City, opt => opt.MapFrom(s => s.Lookup.City))
            .ForMember(d => d.Image, opt => opt.MapFrom(s => s.Lookup.Image))
            .ForMember(d => d.ImageThumbnail, opt => opt.MapFrom(s => s.Lookup.ImageThumbnail))
            .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(s => s.Lookup.PhoneNumber))
            .ForMember(d => d.WhatsappNumber, opt => opt.MapFrom(s => s.Lookup.WhatsappNumber))
            .ForMember(d => d.EmailAddress, opt => opt.MapFrom(s => s.Lookup.EmailAddress))
            .ForMember(d => d.ConversationContactEmail, opt => opt.MapFrom(s => s.Lookup.ConversationContactEmail))
            .ForMember(d => d.ConversationContactWhatsappNumber, opt => opt.MapFrom(s => s.Lookup.ConversationContactWhatsappNumber))
            .ForMember(d => d.Website, opt => opt.MapFrom(s => s.Lookup.Website));
    }
}
