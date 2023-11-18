using System.ComponentModel.DataAnnotations;

namespace AutoHelper.Domain.Entities.Vehicles;

public class VehicleServiceLogVerificationItem: BaseEntity
{
    [Required]
    public ServiceLogVerificationType Type { get; set; } = ServiceLogVerificationType.NotVerified;

    [Required]
    public string CreatedBy { get; set; } = null!;

    public string? PhoneNumber { get; set; } = null!;

    public string? EmailAddress { get; set; } = null!;

}