using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain.Entities.Conversations.Enums;
using AutoHelper.Domain.Entities.Messages;
using AutoHelper.Domain.Entities.Messages.Enums;
using AutoHelper.Domain.Entities.Vehicles;
using AutoHelper.Domain.Enums;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace AutoHelper.WebUI.Controllers;

public class NotificationItemDto : IMapFrom<NotificationItem>
{
    public Guid Id { get; set; }

    public string JobId { get; set; } = null!;

    public DateTime TriggerDate { get; set; }

    public PriorityLevel Priority { get; set; }

    public GeneralNotificationType GeneralType { get; set; }

    public VehicleNotificationType VehicleType { get; set; }

    public ContactType ReceiverContactType { get; set; }

    public string ReceiverContactIdentifier { get; set; } = null!;

    public string VehicleLicensePlate { get; set; } = null!;

    public void Mapping(Profile profile)
    {
        profile.CreateMap<NotificationItem, NotificationItemDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
            .ForMember(d => d.JobId, opt => opt.MapFrom(s => s.JobId))
            .ForMember(d => d.TriggerDate, opt => opt.MapFrom(s => s.TriggerDate))
            .ForMember(d => d.Priority, opt => opt.MapFrom(s => s.Priority))
            .ForMember(d => d.GeneralType, opt => opt.MapFrom(s => s.GeneralType))
            .ForMember(d => d.VehicleType, opt => opt.MapFrom(s => s.VehicleType))
            .ForMember(d => d.ReceiverContactType, opt => opt.MapFrom(s => s.ReceiverContactType))
            .ForMember(d => d.ReceiverContactIdentifier, opt => opt.MapFrom(s => s.ReceiverContactIdentifier))
            .ForMember(d => d.VehicleLicensePlate, opt => opt.MapFrom(s => s.VehicleLicensePlate));
    }
}