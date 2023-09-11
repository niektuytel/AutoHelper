using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AutoHelper.Domain.Entities;

public class GarageServiceItem : BaseAuditableEntity
{
    /// <summary>
    /// UserId of the garage owner
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// GarageId of the garage
    /// </summary>
    public Guid GarageId { get; set; }

    /// <summary>
    /// Like: Inspection, Repair, Maintenance, etc.
    /// </summary>
    public GarageServiceType Type { get; set; } = GarageServiceType.Other;

    /// <summary>
    /// Like: "Change the oil in the engine", "Align the wheels", etc.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Duration of the service in minutes
    /// </summary>
    public int DurationInMinutes { get; set; }

    /// <summary>
    /// Price of the service
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Status of the service
    /// </summary>
    public int Status { get; set; } = -1;

}
