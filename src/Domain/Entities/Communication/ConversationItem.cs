using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AutoHelper.Domain.Common.Enums;
using AutoHelper.Domain.Entities.Communication;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;

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
