using AutoHelper.Domain.Entities.Vehicles;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AutoHelper.Application.Vehicles._DTOs;

public class VehicleServiceLogAttachmentItemOnCreateDto
{
    [Required]
    [StringLength(255)]
    public string FileName { get; set; }

    [Required]
    public byte[] FileData { get; set; }
}