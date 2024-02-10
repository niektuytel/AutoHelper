using AutoHelper.Application.Common.Interfaces;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

namespace AutoHelper.Infrastructure.Services;
internal class AzureBlobStorageService : IBlobStorageService
{
    private const string VehicleAttachmentsContainerName = "vehicle-attachments";
    private const string GarageContainerName = "garage-images";
    private readonly BlobServiceClient _blobServiceClient;

    public AzureBlobStorageService(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("BlobStorageConnection");
        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    public async Task<string> UploadVehicleAttachmentAsync(byte[] fileBytes, string fileExtension, CancellationToken cancellationToken)
    {
        fileExtension = fileExtension.Replace(".", "");
        var blobName = $"{Guid.NewGuid()}.{fileExtension}";

        var containerClient = _blobServiceClient.GetBlobContainerClient(VehicleAttachmentsContainerName);
        await containerClient.CreateIfNotExistsAsync();
        var blobClient = containerClient.GetBlobClient(blobName);

        // Upload the stream to Azure Blob Storage
        using var memoryStream = new MemoryStream(fileBytes);
        var contentInfo = await blobClient.UploadAsync(memoryStream, cancellationToken);

        return blobName;
    }

    public async Task<string> UploadGarageImageAsync(byte[] fileBytes, string fileExtension, CancellationToken cancellationToken)
    {
        fileExtension = fileExtension.Replace(".", "");
        var blobName = $"{Guid.NewGuid()}.{fileExtension}";

        var containerClient = _blobServiceClient.GetBlobContainerClient(GarageContainerName);
        await containerClient.CreateIfNotExistsAsync();
        var blobClient = containerClient.GetBlobClient(blobName);

        // Upload the stream to Azure Blob Storage
        using var memoryStream = new MemoryStream(fileBytes);
        var contentInfo = await blobClient.UploadAsync(memoryStream, cancellationToken);

        return blobName;
    }

}
