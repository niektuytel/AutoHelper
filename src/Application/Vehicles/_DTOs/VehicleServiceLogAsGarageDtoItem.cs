﻿using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;

namespace AutoHelper.Application.Vehicles._DTOs;

public class VehicleServiceLogAsGarageDtoItem : IMapFrom<VehicleServiceLogItem>
{
    public Guid Id { get; set; }
    public string VehicleLicensePlate { get; set; }

    public Guid GarageServiceId { get; set; }
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
        profile.CreateMap<VehicleServiceLogItem, VehicleServiceLogAsGarageDtoItem>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
            .ForMember(d => d.VehicleLicensePlate, opt => opt.MapFrom(s => s.VehicleLicensePlate))
            .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Title))
            .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description))
            .ForMember(d => d.AttachedFile, opt => opt.MapFrom(s => s.AttachedFile))
            .ForMember(d => d.Notes, opt => opt.MapFrom(s => s.Description))
            .ForMember(d => d.Date, opt => opt.MapFrom(s => s.Date))
            .ForMember(d => d.ExpectedNextDate, opt => opt.MapFrom(s => s.ExpectedNextDate))
            .ForMember(d => d.OdometerReading, opt => opt.MapFrom(s => s.OdometerReading))
            .ForMember(d => d.ExpectedNextOdometerReading, opt => opt.MapFrom(s => s.ExpectedNextOdometerReading))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status))
            .ForMember(d => d.MetaData, opt => opt.MapFrom(s => s.ReporterName + " " + s.ReporterPhoneNumber + " " + s.ReporterEmailAddress));
    }
}