﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace AutoHelper.Domain.Entities;

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

    public GarageServicesSettingsItem ServicesSettings { get; set; } = new GarageServicesSettingsItem();

    public ICollection<ContactItem> Contacts { get; set; } = new List<ContactItem>();

    public ICollection<VehicleItem> Vehicles { get; set; } = new List<VehicleItem>();
}
