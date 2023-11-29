using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoHelper.Domain;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AutoHelper.Application.Common.Mappings;
using AutoMapper;

namespace AutoHelper.Application.Vehicles._DTOs;

public class VehicleServiceLogAsGarageDtoItem: IMapFrom<VehicleServiceLogItem>
{
    public Guid Id { get; set; }

    public string VehicleLicensePlate { get; set; }

    public GarageServiceType Type { get; set; } = GarageServiceType.Other;

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
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type))
            .ForMember(d => d.Date, opt => opt.MapFrom(s => s.Date))
            .ForMember(d => d.ExpectedNextDate, opt => opt.MapFrom(s => s.ExpectedNextDate))
            .ForMember(d => d.OdometerReading, opt => opt.MapFrom(s => s.OdometerReading))
            .ForMember(d => d.ExpectedNextOdometerReading, opt => opt.MapFrom(s => s.ExpectedNextOdometerReading))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status))
            .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description))
            .ForMember(d => d.AttachedFile, opt => opt.MapFrom(s => s.AttachedFile))
            .ForMember(d => d.Notes, opt => opt.MapFrom(s => s.Description))
            .ForMember(d => d.MetaData, opt => opt.MapFrom(s => s.ReporterName + " " + s.ReporterPhoneNumber + " " + s.ReporterEmailAddress));
    }
}