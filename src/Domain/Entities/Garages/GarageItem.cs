using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using AutoHelper.Domain.Entities.Vehicles;

namespace AutoHelper.Domain.Entities.Garages;

public class GarageItem : BaseAuditableEntity
{
    /// <summary>
    /// UserId of the garage owner
    /// </summary>
    public string UserId { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public string WhatsAppNumber { get; set; } = "";

    public GarageLocationItem Location { get; set; } = new GarageLocationItem();

    public GarageBankingDetailsItem BankingDetails { get; set; } = new GarageBankingDetailsItem();

    public ICollection<GarageEmployeeItem> Employees { get; set; } = new List<GarageEmployeeItem>();

}
