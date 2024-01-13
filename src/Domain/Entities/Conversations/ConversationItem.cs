﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoHelper.Domain.Entities.Conversations.Enums;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using MediatR;

namespace AutoHelper.Domain.Entities.Conversations;

public class ConversationItem : BaseAuditableEntity
{
    public PriorityLevel Priority { get; set; } = PriorityLevel.Low;

    [Required]
    public string VehicleLicensePlate { get; set; }

    [ForeignKey(nameof(VehicleLicensePlate))]
    public VehicleLookupItem RelatedVehicleLookup { get; set; } = null!;

    [Required]
    public string GarageLookupIdentifier { get; set; }
    
    [ForeignKey(nameof(GarageLookupIdentifier))]
    public GarageLookupItem RelatedGarage { get; set; } = null!;

    [NotMapped]
    public Guid[] RelatedServiceIds
    {
        get
        {
            if (RelatedServiceIdsString == null)
            {
                return Array.Empty<Guid>();
            }

            return RelatedServiceIdsString
                .Split(';')
                .Select(Guid.Parse)
                .ToArray();
        }
        set
        {
            RelatedServiceIdsString = (value == null) ? 
                "" 
                : 
                string.Join(";", value.Select(v => v.ToString()));
        }
    }

    [Required]
    public string RelatedServiceIdsString { get; set; } = "";

    [Required]
    public ConversationType ConversationType { get; set; }

    public ICollection<ConversationMessageItem> Messages { get; set; } = new List<ConversationMessageItem>();

}
