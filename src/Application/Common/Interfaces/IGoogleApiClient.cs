
using AutoHelper.Infrastructure.Common.Models;

namespace AutoHelper.Infrastructure.Services;

public interface IGoogleApiClient
{
    byte[] CreateThumbnail(byte[] originalImage, int thumbnailHeight);
    Task<GoogleApiDetailPlaceItem?> GetPlaceDetailsFromPlaceId(string place_id);
    Task<(byte[]? fileBytes, string fileExtension)> GetPlacePhoto(string photo_reference, int maxWidth);
    Task<string?> SearchPlaceIdFromTextQuery(string queryText);
}