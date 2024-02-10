namespace AutoHelper.Domain.Entities.Vehicles;


public enum VehicleFuelType
{
    /// <summary>
    /// Represents Any fuel type, default.
    /// </summary>
    Any = 0,

    /// <summary>
    /// Represents a diesel fuel type.
    /// </summary>
    Diesel = 1,

    /// <summary>
    /// Represents a gasoline (petrol) fuel type.
    /// </summary>
    Gasoline = 2,

    /// <summary>
    /// Represents a liquefied petroleum gas (LPG) fuel type.
    /// </summary>
    LPG = 3,

    /// <summary>
    /// Represents an electric fuel type.
    /// </summary>
    Electric = 4,

    /// <summary>
    /// Represents a hybrid (combination of gasoline and electric) fuel type.
    /// </summary>
    Hybrid = 5,
}
