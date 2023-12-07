using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain.Entities.Garages;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.HPRtree;
using AutoHelper.Domain.Entities.Conversations.Enums;
using AutoMapper;

namespace AutoHelper.Application.Garages._DTOs;

public class GarageLookupDtoItem : IMapFrom<GarageLookupItem>
{
    public Guid Id { get; set; }

    /// <summary>
    /// Provide a reference to the GarageItem that this GarageSearchableItem is associated with.
    /// When null this GarageSearchableItem is not associated with any GarageItem.
    /// The garage still do not have a GarageItem until the GarageItem is created.
    /// </summary>
    public Guid? GarageId { get; set; }

    public string Identifier { get; set; }

    public string Name { get; set; }

    public string Image { get; set; }

    public string ImageThumbnail { get; set; }

    public int[] DaysOfWeek { get; set; }

    public ICollection<GarageLookupServiceItem> Services { get; set; }

    public string? PhoneNumber { get; set; }

    public string? WhatsappNumber { get; set; }

    public string? EmailAddress { get; set; }

    public string? Website { get; set; }

    public float? Rating { get; set; }

    public int? UserRatingsTotal { get; set; }

    public string Address { get; set; }

    public string City { get; set; }

    /// <summary>
    /// Contact identifier for the conversation with the garage.
    /// This can be a email address or whatsapp number.
    /// When defined, the garage is available for conversation.
    /// </summary>
    public string? ConversationContactEmail { get; set; }
    public string? ConversationContactWhatsappNumber { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<GarageLookupItem, GarageLookupDtoItem>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.GarageId))
            .ForMember(d => d.Image, opt => opt.MapFrom(s => s.Image))
            .ForMember(d => d.ImageThumbnail, opt => opt.MapFrom(s => s.ImageThumbnail))
            .ForMember(d => d.DaysOfWeek, opt => opt.MapFrom(s => s.DaysOfWeek))
            .ForMember(d => d.Services, opt => opt.MapFrom(s => s.Services))
            .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(s => s.PhoneNumber))
            .ForMember(d => d.WhatsappNumber, opt => opt.MapFrom(s => s.WhatsappNumber))
            .ForMember(d => d.EmailAddress, opt => opt.MapFrom(s => s.EmailAddress))
            .ForMember(d => d.Address, opt => opt.MapFrom(s => s.Address))
            .ForMember(d => d.City, opt => opt.MapFrom(s => s.City))
            .ForMember(d => d.ConversationContactEmail, opt => opt.MapFrom(s => s.ConversationContactEmail))
            .ForMember(d => d.ConversationContactWhatsappNumber, opt => opt.MapFrom(s => s.ConversationContactWhatsappNumber));
    }
}