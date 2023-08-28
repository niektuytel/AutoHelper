using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace AutoHelper.Domain.Entities;

public class GarageItem : BaseAuditableEntity
{
    [Required]
    public string Name { get; set; } 

    public LocationItem Location { get; set; } = new LocationItem();

    public BusinessOwnerItem BusinessOwner { get; set; } = new BusinessOwnerItem();

    public BankingInfoItem BankingDetails { get; set; } = new BankingInfoItem();

    public ICollection<ContactItem> Contacts { get; set; } = new List<ContactItem>();

    public ICollection<GarageEmployeeItem> Employees { get; set; } = new List<GarageEmployeeItem>();

    public ICollection<GarageServiceItem> Services { get; set; } = new List<GarageServiceItem>();

    public ICollection<VehicleItem> Vehicles { get; set; } = new List<VehicleItem>();

}
