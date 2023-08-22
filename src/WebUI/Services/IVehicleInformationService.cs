using WebUI.Models.Response;

namespace WebUI.Services;
public interface IVehicleInformationService
{
    Task<bool> ValidVehicle(string licensePlate);
    Task<VehicleInformationResponse> GetVehicleInformationAsync(string licensePlate);
}