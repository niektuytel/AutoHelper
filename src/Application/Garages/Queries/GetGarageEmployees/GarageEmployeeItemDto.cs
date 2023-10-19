using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Application.Garages.Queries.GetGaragesLookups;
using AutoHelper.Domain.Entities.Garages;
using AutoMapper;

namespace AutoHelper.Application.Garages.Queries.GetGarageEmployees;

public class GarageEmployeeItemDto : IMapFrom<GarageEmployeeItem>
{
    public Guid Id { get; set; }

    /// <summary>
    /// Employee is active or not (if not, he is not visible in the garage)
    /// </summary>
    public bool IsActive { get; set; } = false;

    /// <summary>
    /// Man power contact information
    /// </summary>
    public GarageEmployeeContactItem Contact { get; set; }

    /// <summary>
    /// Working schema for this user
    /// </summary>
    public IEnumerable<GarageEmployeeWorkSchemaItem> WorkSchema { get; set; }

    /// <summary>
    /// All the experiences of this user
    /// </summary>
    public IEnumerable<GarageEmployeeWorkExperienceItem> WorkExperiences { get; set; }


    public void Mapping(Profile profile)
    {
        profile.CreateMap<GarageEmployeeItem, GarageEmployeeItemDto>()
            .ForMember(dest => dest.Contact, opt => opt.MapFrom(src => src.Contact))
            .ForMember(dest => dest.WorkSchema, opt => opt.MapFrom(src => src.WorkSchema))
            .ForMember(dest => dest.WorkExperiences, opt => opt.MapFrom(src => src.WorkExperiences));
    }

}
