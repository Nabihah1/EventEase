using Azure.Storage.Blobs;

namespace EventEase.Services
{
    public interface IBlobService
    {
        Task<string> UploadFileAsync(Stream file, string fileName, string contentType);
    }

    public class BlobService : IBlobService
    {
        private readonly BlobContainerClient _container;

        public BlobService(IConfiguration cfg)
        {
            // appsettings.json -->storage --> has the connection string and container name 
            var connectionString = cfg["Storage:ConnectionString"];
            var containerName = cfg["Storage:Container"];

            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(containerName))
                throw new InvalidOperationException("Blob connection string or container name is missing.");

            _container = new BlobContainerClient(connectionString, containerName);
            _container.CreateIfNotExists(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
        }
               

        public async Task<string> UploadFileAsync(Stream file, string fileName, string contentType)
        {
            var blob = _container.GetBlobClient(fileName);
            await blob.UploadAsync(file, new Azure.Storage.Blobs.Models.BlobHttpHeaders { ContentType = contentType });
            return blob.Uri.ToString(); 
        }
    }
}
