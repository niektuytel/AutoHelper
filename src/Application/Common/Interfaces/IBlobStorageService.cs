namespace AutoHelper.Application.Common.Interfaces;
public interface IBlobStorageService
{
    Task<string> UploadVehicleAttachmentAsync(byte[] fileBytes, string fileExtension, CancellationToken cancellationToken);
    Task<string> UploadGarageImageAsync(byte[] fileBytes, string fileExtension, CancellationToken cancellationToken);
}
