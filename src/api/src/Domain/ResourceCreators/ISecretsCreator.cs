namespace Domain.Interfaces
{
    public interface ISecretsCreator
    {
        Task AddSecrets(string keyVaultName, IDictionary<string, string> secrets);
    }
}
