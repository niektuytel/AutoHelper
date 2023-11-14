using System.ComponentModel.DataAnnotations;

namespace AutoHelper.Domain.Entities.Vehicles;

public class VehicleServiceLogVerificationItem: BaseAuditableEntity
{
    [Required]
    public ServiceLogVerificationType Type { get; set; } = ServiceLogVerificationType.NotVerified;


}