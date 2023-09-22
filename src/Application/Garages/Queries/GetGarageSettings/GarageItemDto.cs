using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Domain.Entities;

namespace AutoHelper.Application.Garages.Queries.GetGarageSettings;

public class GarageItemDto : IMapFrom<GarageItem>
{
    public string Name { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public string WhatsAppNumber { get; set; } = "";

    public GarageLocationItem Location { get; set; } = new GarageLocationItem();

    public GarageBankingDetailsItem BankingDetails { get; set; } = new GarageBankingDetailsItem();

    public GarageServicesSettingsItem ServicesSettings { get; set; } = new GarageServicesSettingsItem();

    public ICollection<ContactItem> Contacts { get; set; } = new List<ContactItem>();
}
