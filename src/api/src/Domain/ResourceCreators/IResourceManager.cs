namespace Domain.Interfaces
{
    public interface IResourceManager
    {
        public string Name { get; }
        Task<string> GetResourceSecret(string resourceGroupName, string resourceName, CancellationToken ct);
    }
}
