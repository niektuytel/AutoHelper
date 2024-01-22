﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Conversations.Enums;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Messages.Enums;
using AutoHelper.Domain.Entities.Vehicles;
using MediatR;

namespace AutoHelper.Domain.Entities.Messages;

public class NotificationItem : BaseAuditableEntity
{
    /// <summary>
    /// Can been null if the notification is not recurring
    /// </summary>
    public string? Cron { get; set; } = null;

    /// <summary>
    /// How important is this notification
    /// </summary>
    public PriorityLevel Priority { get; set; } = PriorityLevel.Low;

    /// <summary>
    /// What type of notification is this
    /// </summary>
    [Required]
    public NotificationType Type { get; set; }

    /// <summary>
    /// Type of receiver, can be whatsapp, email, etc
    /// </summary>
    [Required]
    public ContactType ReceiverContactType { get; set; }

    /// <summary>
    /// Identifier of the receiver, so we know where to send the notification
    /// </summary>
    [Required]
    public string ReceiverContactIdentifier { get; set; } = null!;

    /// <summary>
    /// The license plate of the vehicle that this notification is related to
    /// </summary>
    [Required]
    public string VehicleLicensePlate { get; set; } = null!;

    [ForeignKey(nameof(VehicleLicensePlate))]
    public VehicleLookupItem RelatedVehicleLookup { get; set; } = null!;

}
