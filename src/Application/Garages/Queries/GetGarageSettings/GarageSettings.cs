using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain.Entities;

namespace AutoHelper.Application.Garages.Queries.GetGarageSettings;

public class GarageSettings : IMapFrom<GarageItem>
{
    [Required]
    public string Name { get; set; }

    public LocationItem Location { get; set; } = new LocationItem();

    public BusinessOwnerItem BusinessOwner { get; set; } = new BusinessOwnerItem();

    public BankingInfoItem BankingDetails { get; set; } = new BankingInfoItem();

    public ICollection<ContactItem> Contacts { get; set; } = new List<ContactItem>();

}
