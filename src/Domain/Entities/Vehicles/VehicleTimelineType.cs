namespace AutoHelper.Domain.Entities.Vehicles;

public enum VehicleTimelineType
{
    Unknown = 0,
    SucceededMOT = 1,
    FailedMOT = 2,
    Service = 3,
    Repair = 4,
    OwnerChange = 5
}