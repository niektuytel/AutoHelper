using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AutoHelper.Domain.Entities.Conversations;

namespace AutoHelper.Domain.Entities.Garages;

public class GarageItem : BaseAuditableEntity
{
    /// <summary>
    /// UserId of the garage owner
    /// </summary>
    [Required]
    public string UserId { get; set; } = null!;

    /// <summary>
    /// Email of the garage owner
    /// </summary>
    [Required]
    public string UserEmail { get; set; } = null!;

    [Required]
    public string GarageLookupIdentifier { get; set; } = null!;

    [ForeignKey(nameof(GarageLookupIdentifier))]
    public virtual GarageLookupItem Lookup { get; set; } = null!;

    public ICollection<GarageServiceItem> Services { get; set; } = new List<GarageServiceItem>();

    [Required]
    public ICollection<ConversationItem> Conversations { get; set; } = new List<ConversationItem>();
}
