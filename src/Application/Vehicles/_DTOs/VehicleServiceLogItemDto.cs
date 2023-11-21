using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using System.Net.Mail;
using AutoMapper;

namespace AutoHelper.Application.Vehicles._DTOs;


public class VehicleServiceLogItemDto : IMapFrom<VehicleServiceLogItem>
{
    public string GarageLookupName { get; set; }
    public string GarageLookupIdentifier { get; set; }
    public GarageServiceType Type { get; set; }
    public DateTime Date { get; set; }
    public int OdometerReading { get; set; }
    public string? Description { get; set; }
    public string? AttachedFile { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<VehicleServiceLogItem, VehicleServiceLogItemDto>()
            .ForMember(d => d.GarageLookupName, opt => opt.MapFrom(s => s.GarageLookup!.Name))
            .ForMember(d => d.GarageLookupIdentifier, opt => opt.MapFrom(s => s.GarageLookupIdentifier))
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type))
            .ForMember(d => d.Date, opt => opt.MapFrom(s => s.Date))
            .ForMember(d => d.OdometerReading, opt => opt.MapFrom(s => s.OdometerReading))
            .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description))
            .ForMember(d => d.AttachedFile, opt => opt.MapFrom(s => s.AttachedFile));
    }
}

