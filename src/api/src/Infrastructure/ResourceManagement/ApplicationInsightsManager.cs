using Domain.Interfaces;
using Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;

namespace Infrastructure.ResourceManagement
{
    public class ApplicationInsightsManager : IResourceManager
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApplicationInsightsManager> _logger;

        private readonly AzureAdOptions _options;
        private readonly string _subscriptionId;

        public ApplicationInsightsManager(HttpClient httpClient, IOptions<AzureAdOptions> options, IConfiguration configuration, ILogger<ApplicationInsightsManager> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _subscriptionId = configuration.GetSection("SubscriptionId").Get<string>();
            _logger = logger;
        }

        public string Name => "Microsoft.Insights/Components";

        public async Task<string> GetResourceSecret(string resourceGroupName, string resourceName, CancellationToken ct)
        {
            try
            {
                var cc = new ClientCredential(_options.ClientId, _options.ClientSecret);
                var context = new AuthenticationContext("https://login.microsoftonline.com/" + _options.TenantId);
#pragma warning disable CS0618 // Type or member is obsolete
                var result = await context.AcquireTokenAsync("https://management.azure.com/", cc);
#pragma warning restore CS0618 // Type or member is obsolete

                _httpClient.BaseAddress = new Uri($"https://management.azure.com/subscriptions/{_subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Insights/Components/{resourceName}?api-version=2020-02-02");
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + result.AccessToken);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, _httpClient.BaseAddress);
                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var stringResponse = await response.Content.ReadAsStringAsync();
                    JObject objectRespone = JObject.Parse(stringResponse);

                    return objectRespone["properties"]["ConnectionString"].ToString();
                }

                return string.Empty;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "ExceptionMessage: {exceptionMessage}", ex.Message);

                return string.Empty;
            }
        }

    }
}
