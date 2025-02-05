﻿using System.ComponentModel.DataAnnotations;

namespace AutoHelper.Domain.Entities.Vehicles;

public class VehicleLookupItem
{
    public VehicleLookupItem()
    { }

    [Key]
    [Required]
    public string LicensePlate { get; set; }

    [Required]
    public DateTime? DateOfMOTExpiry { get; set; }

    [Required]
    public DateTime? DateOfAscription { get; set; }

    [Required]
    public DateTime Created { get; set; }

    public string? CreatedBy { get; set; }

    [Required]
    public DateTime? LastModified { get; set; }

    public string? LastModifiedBy { get; set; }

    [Required]
    public List<VehicleTimelineItem> Timeline { get; set; } = new List<VehicleTimelineItem>();

    [Required]
    public ICollection<VehicleServiceLogItem> ServiceLogs { get; set; } = new List<VehicleServiceLogItem>();

}
