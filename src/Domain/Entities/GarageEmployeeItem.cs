namespace AutoHelper.Domain.Entities;

public class GarageEmployeeItem : BaseAuditableEntity
{
    /// <summary>
    /// UserId of the garage account
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// GarageId of the garage (currently working for)
    /// </summary>
    public Guid GarageId { get; set; }

    /// <summary>
    /// Employee is active or not (if not, he is not visible in the garage)
    /// </summary>
    public bool IsActive { get; set; } = false;

    /// <summary>
    /// Man power contact information
    /// </summary>
    public ContactItem Contact { get; set; }

    /// <summary>
    /// Working schema for this user
    /// </summary>
    public IEnumerable<GarageEmployeeWorkSchemaItem> WorkSchema { get; set; }

    /// <summary>
    /// All the experiences of this user
    /// </summary>
    public IEnumerable<GarageEmployeeWorkExperienceItem> WorkExperiences { get; set; }

}