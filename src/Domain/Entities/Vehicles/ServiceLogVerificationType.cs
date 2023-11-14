namespace AutoHelper.Domain;

public enum ServiceLogVerificationType
{
    /// <summary>
    /// The service log item is not verified.
    /// </summary>
    NotVerified = 0,

    /// <summary>
    /// The service log item is verified by the garage.
    /// </summary>
    VerifiedByGarage = 1,

    /// <summary>
    /// The service log item is verified by the vehicle owner.
    /// </summary>
    VerifiedByOwner = 2,

    /// <summary>
    /// The service log item is verified by the vehicle owner and the garage.
    /// </summary>
    VerifiedByBoth = 3
}