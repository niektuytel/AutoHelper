using System.ComponentModel.DataAnnotations;

namespace AutoHelper.Application.Vehicles._DTOs;

public class VehicleServiceLogAttachmentDtoItem
{
    [Required]
    [StringLength(255)]
    public string FileName { get; set; }

    [Required]
    public byte[] FileData { get; set; }
}