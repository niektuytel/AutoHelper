namespace AutoHelper.Domain;

public enum VehicleServiceLogStatus
{
    /// <summary>
    /// The service log item is not verified.
    /// </summary>
    NotVerified = 0,

    /// <summary>
    /// The service log item is verified by the garage.
    /// </summary>
    VerifiedByGarage = 1,

}