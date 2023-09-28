using System.ComponentModel.DataAnnotations;

namespace AutoHelper.Domain.Entities.Garages;

public class GarageEmployeeWorkSchemaItem : BaseEntity
{
    [Required]
    public Guid EmployeeId { get; set; }

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