namespace Domain.Interfaces
{
    public interface IConfigurationRepository
    {
        public Task<Configuration> GetConfigurationAsync(CancellationToken ct);
    }
}
