using Domain.Interfaces;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Options;
using Infrastructure.Options;

namespace Infrastructure.ResourceCreators
{
    public class KeyVaultSecretsCreator : ISecretsCreator
    {
        private readonly AzureAdOptions _azureAdOptions;

        public KeyVaultSecretsCreator(IOptions<AzureAdOptions> azureAdOptions)
        {
            _azureAdOptions = azureAdOptions.Value;
        }

        public async Task AddSecrets(string keyVaultName, IDictionary<string, string> secrets)
        {
            var kvUri = "https://" + keyVaultName + ".vault.azure.net";

            var client = new SecretClient(new Uri(kvUri),
                new ClientSecretCredential(_azureAdOptions.TenantId, _azureAdOptions.ClientId, _azureAdOptions.ClientSecret));

            foreach (var secret in secrets)
            {
                await client.SetSecretAsync(secret.Key, secret.Value);
            }
        }
    }
}
