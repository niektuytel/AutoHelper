using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AutoHelper.Domain.Entities.Garages;

namespace AutoHelper.Domain.Entities.Vehicles;

public class VehicleServiceLogItem: BaseAuditableEntity
{
    public VehicleServiceLogItem()
    {
        ServiceItems = new List<GarageServiceItem>();
    }

    [Required]
    public Guid VehicleLookupId { get; set; }
    
    [ForeignKey(nameof(VehicleLookupId))]
    public VehicleLookupItem VehicleLookup { get; set; }
    
    [Required]
    public DateTime ServiceDate { get; set; }

    // Optional: Expected next service date based on average usage or time
    public DateTime? ExpectedNextServiceDate { get; set; }

    [Required]
    public int OdometerReading { get; set; }

    public decimal? TotalCost { get; set; }

    public string? WorkDescription { get; set; }

    public string? PerformedBy { get; set; }

    public string? Notes { get; set; }

    // Optional: URL to documentation or related images
    public string? DocumentationUrl { get; set; }

    [Required]
    public ICollection<GarageServiceItem> ServiceItems { get; set; }

    public string MetaData { get; set; } = "";
}

