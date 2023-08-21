using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO.Compression;

namespace Infrastructure.Persistance.Blob
{
    public class BlobManager : IBlobManager
    {
        private readonly BlobServiceClient _blobServiceClient;
        public BlobManager(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task ExtractZipBlobToDirectory(string container, string fileName, string directoryName, CancellationToken ct)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(container);
            var blobClient = containerClient.GetBlobClient(fileName);
            if (await blobClient.ExistsAsync(ct))
            {
                var response = await blobClient.DownloadAsync(ct);
                using ZipArchive archive = new(response.Value.Content);
                archive.ExtractToDirectory(directoryName);
            }
            else
            {
                throw new Exception("File dosen't exists");
            }
        }

        public Task<string> GetFileContentAsync(string container, string fileName, CancellationToken ct)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(container);

            return GetFileContent(containerClient, fileName, ct);
        }

        public async Task<IEnumerable<string>> GetFilesContentAsync(string container, CancellationToken ct)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(container);
            var result = new List<string>();
 
            await foreach (BlobItem blob in containerClient.GetBlobsAsync(cancellationToken: ct))
            {
                result.Add(await GetFileContent(containerClient, blob.Name, ct));
            }

            return result;
        }

        private static async Task<string> GetFileContent(BlobContainerClient containerClient, string fileName, CancellationToken ct)
        {
            var result = string.Empty;
            var blobClient = containerClient.GetBlobClient(fileName);
            if (await blobClient.ExistsAsync(ct))
            {
                var response = await blobClient.DownloadAsync(ct);
                using var streamReader = new StreamReader(response.Value.Content);
                result = streamReader.ReadToEnd();
            }

            return result;
        }
    }
}
