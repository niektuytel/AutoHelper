using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AutoHelper.Domain.Entities;

public class GarageServiceItem : BaseAuditableEntity
{
    /// <summary>
    /// Like "Oil Change", "Wheel Alignment", etc.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Like: "Change the oil in the engine", "Align the wheels", etc.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Duration of the service in minutes
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    /// Price of the service
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Status of the service
    /// </summary>
    public int Status { get; set; } = -1;

    public Guid GarageId { get; set; }

    [ForeignKey("GarageId")]
    public GarageItem Garage { get; set; }
}
