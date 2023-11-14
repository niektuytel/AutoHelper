using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AutoHelper.Domain.Entities.Vehicles;

public class VehicleServiceAttachmentItem : BaseEntity
{
    [Required]
    public Guid VehicleServiceLogItemId { get; set; }

    [ForeignKey(nameof(VehicleServiceLogItemId))]
    public VehicleServiceLogItem VehicleServiceLogItem { get; set; }

    [Required]
    [StringLength(255)]
    public string FileName { get; set; }

    [Required]
    public string FileType { get; set; }

    [Required]
    public long FileSize { get; set; }

    [Required]
    public string ContainerName { get; set; }

    public string Description { get; set; }

}