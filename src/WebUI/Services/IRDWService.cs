using Newtonsoft.Json.Linq;

namespace WebUI.Services;

public interface IRDWService
{
    Task<JToken?> GetVehicle(string licensePlate);
    Task<JArray?> GetVehicleShafts(string licensePlate);
    Task<JToken?> GetVehicleFuel(string licensePlate);
    string GetCounterReadingsDescription(string judgement);
    Task<bool> VehicleExist(string licensePlate);
}
