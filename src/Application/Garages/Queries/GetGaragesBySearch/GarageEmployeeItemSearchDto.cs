using System.ComponentModel.DataAnnotations.Schema;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain.Entities.Deprecated;
using AutoHelper.Domain.Entities.Garages;

namespace AutoHelper.Application.Garages.Queries.GetGaragesBySearch;

public class GarageEmployeeItemSearchDto : IMapFrom<GarageEmployeeItem>
{
    public ICollection<GarageEmployeeWorkExperienceItem> WorkExperiences { get; set; }

    public ICollection<int> WorkingDaysOfWeek { get; set; }


}