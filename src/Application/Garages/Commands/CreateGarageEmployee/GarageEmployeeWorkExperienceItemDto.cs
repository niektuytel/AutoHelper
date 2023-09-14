using System.ComponentModel.DataAnnotations;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain.Entities;

namespace AutoHelper.Application.Garages.Commands.CreateGarageEmployee;

public class GarageEmployeeWorkExperienceItemDto
{
    [Required]
    public Guid ServiceId { get; set; }

    [Required]
    public string Description { get; set; }

}