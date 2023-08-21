using Microsoft.Graph;

namespace Azure.Infrastructure.IdentityManagement
{
    internal class IdentityManagement : IIdentityManagement
    {
        private readonly GraphServiceClient _graphClient;

        public IdentityManagement(GraphServiceClient graphClient)
        {
            _graphClient = graphClient;
        }

        public async Task<string> GetServicePrinicpalNameByAppIdAsync(string identifier)
        {
            var servicePrincipal = await _graphClient.ServicePrincipals
            .Request()
            .Filter($"appId eq '{identifier}'")
            .Select("displayName")
            .Top(1)
            .GetAsync();
     
            return servicePrincipal.First().DisplayName;
        }

        public async Task<bool> ServicePrinicpalExistsInGroupAsync(string identifier, string groupIdentifier)
        {
            var servicePrincipal = await _graphClient.ServicePrincipals
            .Request()
            .Filter($"appId eq '{identifier}'")
            .Select("id")
            .Top(1)
            .GetAsync();

            var groups = new List<string>();
            var groupsResponse = await _graphClient.ServicePrincipals[servicePrincipal.First().Id].MemberOf
                .Request()
                .GetAsync();

            while(groupsResponse.Count > 0)
            {
                foreach (Group g in groupsResponse)
                {
                    groups.Add(g.Id);
                }
                if (groupsResponse.NextPageRequest != null)
                {
                    groupsResponse = await groupsResponse.NextPageRequest.GetAsync();
                }
                else
                {
                    break;
                }
            }

            return groups.Any(x => x == groupIdentifier);
        }
    }
}
