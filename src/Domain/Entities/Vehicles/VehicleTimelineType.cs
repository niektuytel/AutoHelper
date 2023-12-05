namespace AutoHelper.Domain.Entities.Vehicles;

public enum VehicleTimelineType
{
    Unknown = 0,
    Service = 1,
    Repair = 2,
    Inspection = 3,

    // From RDW
    SucceededMOT = 400,
    FailedMOT = 401,
    OwnerChange = 402
}