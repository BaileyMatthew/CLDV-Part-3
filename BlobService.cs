using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EventBooking.Services
{
    public class BlobStorageService
    {
        private readonly string _connectionString = "DefaultEndpointsProtocol=https;AccountName=st10445046;AccountKey=jT53/9pz5NppAvAJ5FUi016b1DZNVUU7OJmvjQyqqwxzaq5staL3aQt+um0gukQSdbjlKluMMo00+AStZXJ7Ag==;EndpointSuffix=core.windows.net";
        private readonly string _containerName = "eventimages";

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync();

            var blobClient = containerClient.GetBlobClient(Guid.NewGuid() + Path.GetExtension(file.FileName));

            await using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, overwrite: true);

            return blobClient.Uri.ToString(); // Return the public URL
        }
    }
}
