using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using NetTopologySuite.Geometries;

namespace AutoHelper.Application.Vehicles._DTOs;

public class VehicleLookupDtoItem : IMapFrom<VehicleLookupItem>
{
    public VehicleLookupDtoItem()
    { }

    public string LicensePlate { get; set; }

    public DateTime DateOfMOTExpiry { get; set; }

    public Geometry Location { get; set; }

    public string? ReporterPhoneNumber { get; set; }

    public string? ReporterWhatsappNumber { get; set; }

    public string? ReporterEmailAddress { get; set; }

    public void Mapping(Profile profile)
    {         
        profile.CreateMap<VehicleLookupItem, VehicleLookupDtoItem>()
            .ForMember(d => d.LicensePlate, opt => opt.MapFrom(s => s.LicensePlate))
            .ForMember(d => d.DateOfMOTExpiry, opt => opt.MapFrom(s => s.DateOfMOTExpiry))
            .ForMember(d => d.Location, opt => opt.MapFrom(s => s.Location))
            .ForMember(d => d.ReporterPhoneNumber, opt => opt.MapFrom(s => s.ReporterPhoneNumber))
            .ForMember(d => d.ReporterWhatsappNumber, opt => opt.MapFrom(s => s.ReporterWhatsappNumber))
            .ForMember(d => d.ReporterEmailAddress, opt => opt.MapFrom(s => s.ReporterEmailAddress));
       }
}
