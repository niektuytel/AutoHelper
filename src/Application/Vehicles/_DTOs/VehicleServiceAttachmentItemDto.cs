using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain.Entities.Vehicles;

namespace AutoHelper.Application.Vehicles._DTOs;

public class VehicleServiceAttachmentItemDto: IMapFrom<VehicleServiceAttachmentItem>
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