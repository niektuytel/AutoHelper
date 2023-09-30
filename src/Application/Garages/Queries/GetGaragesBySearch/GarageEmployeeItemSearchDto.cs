using System.ComponentModel.DataAnnotations.Schema;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain.Entities.Deprecated;
using AutoHelper.Domain.Entities.Garages;

namespace AutoHelper.Application.Garages.Queries.GetGaragesBySearch;

public class GarageEmployeeItemSearchDto : IMapFrom<GarageEmployeeItem>
{
    public bool IsActive { get; set; } = false;

    public ICollection<GarageEmployeeWorkExperienceItem> WorkExperiences { get; set; }

}