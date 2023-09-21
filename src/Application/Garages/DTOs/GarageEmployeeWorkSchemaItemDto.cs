using System.ComponentModel.DataAnnotations;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain.Entities;

namespace AutoHelper.Application.Garages.DTOs;

public class GarageEmployeeWorkSchemaItemDto
{
    public int WeekOfYear { get; set; } = -1;

    [Required]
    public int DayOfWeek { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [StringLength(500)]
    public string Notes { get; set; } = "";

}