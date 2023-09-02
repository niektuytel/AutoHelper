using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace AutoHelper.Domain.Entities;

public class GarageItem : BaseAuditableEntity
{
    public string Name { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public string WhatsAppNumber { get; set; } = "";

    public LocationItem Location { get; set; } = new LocationItem();

    public BankingDetailsItem BankingDetails { get; set; } = new BankingDetailsItem();

    public ICollection<ContactItem> Contacts { get; set; } = new List<ContactItem>();

    public ICollection<GarageEmployeeItem> Employees { get; set; } = new List<GarageEmployeeItem>();

    public ICollection<GarageServiceItem> Services { get; set; } = new List<GarageServiceItem>();

    public ICollection<VehicleItem> Vehicles { get; set; } = new List<VehicleItem>();

}
