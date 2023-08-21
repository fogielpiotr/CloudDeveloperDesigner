using Domain.Interfaces;
using Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;

namespace Infrastructure.ResourceManagement
{
    public class SignalRManager : IResourceManager
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SignalRManager> _logger;

        private readonly AzureAdOptions _options;
        private readonly string _subscriptionId;


        public SignalRManager(HttpClient httpClient, IOptions<AzureAdOptions> options, IConfiguration configuration, ILogger<SignalRManager> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _subscriptionId = configuration.GetSection("SubscriptionId").Get<string>();
            _logger = logger;
        }

        public string Name => "Microsoft.SignalRService/SignalR";
        public async Task<string> GetResourceSecret(string resourceGroupName, string resourceName, CancellationToken ct)
        {
            try
            {
                var cc = new ClientCredential(_options.ClientId, _options.ClientSecret);
                var context = new AuthenticationContext("https://login.microsoftonline.com/" + _options.TenantId);
#pragma warning disable CS0618 // Type or member is obsolete
                var result = await context.AcquireTokenAsync("https://management.azure.com/", cc);
#pragma warning restore CS0618 // Type or member is obsolete

                _httpClient.BaseAddress = new Uri($"https://management.azure.com/subscriptions/{_subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.SignalRService/signalR/{resourceName}/listKeys?api-version=2021-09-01-preview");
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + result.AccessToken);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _httpClient.BaseAddress);
                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var stringResponse = await response.Content.ReadAsStringAsync();
                    JObject objectRespone = JObject.Parse(stringResponse);

                    return objectRespone["primaryConnectionString"].ToString();
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExceptionMessage: {exceptionMessage}", ex.Message);

                return string.Empty;
            }

        }
    }
}
