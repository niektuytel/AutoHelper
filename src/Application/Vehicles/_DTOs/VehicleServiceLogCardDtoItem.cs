﻿using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;

namespace AutoHelper.Application.Vehicles._DTOs;


public class VehicleServiceLogCardDtoItem : IMapFrom<VehicleServiceLogItem>
{
    public Guid Id { get; set; }
    public string GarageLookupName { get; set; }
    public string GarageLookupIdentifier { get; set; }

    public GarageServiceType Type { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? AttachedFile { get; set; }
    public string Notes { get; set; } = "";

    public DateTime Date { get; set; }
    public DateTime? ExpectedNextDate { get; set; } = null!;
    public int OdometerReading { get; set; }
    public int? ExpectedNextOdometerReading { get; set; } = null!;

    public VehicleServiceLogStatus Status { get; set; } = VehicleServiceLogStatus.NotVerified;
    public string MetaData { get; set; } = "";

    public void Mapping(Profile profile)
    {
        profile.CreateMap<VehicleServiceLogItem, VehicleServiceLogCardDtoItem>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
            .ForMember(d => d.GarageLookupName, opt => opt.MapFrom(s => s.GarageLookup!.Name))
            .ForMember(d => d.GarageLookupIdentifier, opt => opt.MapFrom(s => s.GarageLookupIdentifier))
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type))
            .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Title))
            .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description))
            .ForMember(d => d.AttachedFile, opt => opt.MapFrom(s => s.AttachedFile))
            .ForMember(d => d.Notes, opt => opt.MapFrom(s => s.Notes))
            .ForMember(d => d.Date, opt => opt.MapFrom(s => s.Date))
            .ForMember(d => d.ExpectedNextDate, opt => opt.MapFrom(s => s.ExpectedNextDate))
            .ForMember(d => d.OdometerReading, opt => opt.MapFrom(s => s.OdometerReading))
            .ForMember(d => d.ExpectedNextOdometerReading, opt => opt.MapFrom(s => s.ExpectedNextOdometerReading))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status))
            .ForMember(d => d.MetaData, opt => opt.MapFrom(s => s.MetaData))
        ;
    }
}

