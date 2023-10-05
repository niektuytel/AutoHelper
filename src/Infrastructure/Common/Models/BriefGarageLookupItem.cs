using System.ComponentModel.DataAnnotations;

namespace AutoHelper.Infrastructure.Common.Interfaces;

public class BriefGarageLookupItem
{
    public string Identifier { get; set; }

    public string Name { get; set; }

    public string Address { get; set; }

    public string City { get; set; }
}