namespace Infrastructure.Persistance.Blob
{
    public interface IBlobManager
    {
        Task ExtractZipBlobToDirectory(string container, string fileName, string directoryName, CancellationToken ct);
        Task<string> GetFileContentAsync(string container, string fileName, CancellationToken ct);
        Task<IEnumerable<string>> GetFilesContentAsync(string container, CancellationToken ct);
    }
}
