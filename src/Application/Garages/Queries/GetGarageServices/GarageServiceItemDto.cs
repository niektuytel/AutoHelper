using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain.Entities;

namespace AutoHelper.Application.Garages.Queries.GetGarageServices;
public class GarageServiceItemDto : IMapFrom<GarageServiceItem>
{
    public Guid Id { get; set; }

    /// <summary>
    /// Type of the service
    /// </summary>
    public GarageServiceType Type { get; set; }

    /// <summary>
    /// Like: "Change the oil in the engine", "Align the wheels", etc.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Duration of the service in minutes
    /// </summary>
    public int DurationInMinutes { get; set; }

    /// <summary>
    /// Price of the service
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Status of the service
    /// </summary>
    public int Status { get; set; } = -1;

}
