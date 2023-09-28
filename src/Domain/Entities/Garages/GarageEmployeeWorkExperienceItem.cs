using System.ComponentModel.DataAnnotations;

namespace AutoHelper.Domain.Entities.Garages;

public class GarageEmployeeWorkExperienceItem : BaseEntity
{
    [Required]
    public Guid EmployeeId { get; set; }

    [Required]
    public Guid ServiceId { get; set; }

    [Required]
    public string Description { get; set; }

}