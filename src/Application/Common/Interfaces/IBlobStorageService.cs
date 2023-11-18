using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AutoHelper.Application.Common.Interfaces;
public interface IBlobStorageService
{
    Task<string> UploadVehicleAttachmentAsync(byte[] fileBytes, string fileExtension, CancellationToken cancellationToken);
    Task<string> UploadGarageImageAsync(byte[] fileBytes, string fileExtension, CancellationToken cancellationToken);
}
