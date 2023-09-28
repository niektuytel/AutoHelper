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
    public Guid? OwnerId { get; set; }

    [ForeignKey("OwnerId")]
    public VehicleOwnerItem Owner { get; set; }

    [Required]
    public Guid VehicleId { get; set; }
    
    [ForeignKey("VehicleId")]
    public VehicleItem Vehicle { get; set; }
    
    [Required]
    public DateTime Date { get; set; }

    [Required]
    public int Mileage { get; set; }

    [Required]
    public decimal TotalPrice { get; set; }

    public string? Description { get; set; }

    [Required]
    public ICollection<GarageServiceItem> ServiceItems { get; set; }

    public string MetaData { get; set; } = "";
}