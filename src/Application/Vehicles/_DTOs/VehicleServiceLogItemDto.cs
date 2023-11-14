using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using System.Net.Mail;

namespace AutoHelper.Application.Vehicles._DTOs;


public class VehicleServiceLogItemDto : IMapFrom<VehicleServiceLogItem>
{
    public string VehicleLicensePlate { get; set; }

    public Guid PerformedByGarageId { get; set; }

    [Required]
    public GarageServiceType Type { get; set; } = GarageServiceType.Other;

    [Required]
    public DateTime Date { get; set; }

    public DateTime? ExpectedNextDate { get; set; } = null!;

    [Required]
    public int OdometerReading { get; set; }

    public int? ExpectedNextOdometerReading { get; set; } = null!;

    public string? Description { get; set; }

    public string? Notes { get; set; }

    public ICollection<VehicleServiceAttachmentItemDto> Attachments { get; set; }

    public string MetaData { get; set; } = "";
}

