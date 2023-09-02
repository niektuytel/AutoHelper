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

public class GarageSettings : IMapFrom<GarageItem>
{
    public string Name { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public string WhatsAppNumber { get; set; } = "";


    public LocationItem Location { get; set; } = new LocationItem();

    public BankingDetailsItem BankingDetails { get; set; } = new BankingDetailsItem();

    public ICollection<ContactItem> Contacts { get; set; } = new List<ContactItem>();
}
