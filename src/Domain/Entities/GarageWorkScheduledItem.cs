using System.ComponentModel.DataAnnotations;

namespace AutoHelper.Domain.Entities;

public class GarageWorkScheduledItem: BaseEntity
{
    [Required]
    public Guid EmployeeId { get; set; }

    [Required]
    public Guid ServiceId { get; set; }

    [Required]
    [StringLength(100)]
    public string TaskDescription { get; set; }

    [Required]
    public DateTime StartDateTime { get; set; }

    [Required]
    public DateTime EndDateTime { get; set; }

    public bool IsAllDayEvent { get; set; }

    [StringLength(500)]
    public string Notes { get; set; }

}