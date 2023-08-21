using Application.Common.Interfaces;
using Azure.Infrastructure.IdentityManagement;
using Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Worker.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IIdentityManagement _identityManagement;
        private readonly string _groupId;
        private readonly string _appId;

        public CurrentUserService(IIdentityManagement identityManagement, IOptions<AzureAdOptions> azureAdOptions)
        {
            _identityManagement = identityManagement;
            _groupId = azureAdOptions.Value.GroupId;
            _appId = azureAdOptions.Value.ClientId;
        }

        public Task<string> GetUserName()
        {
            return _identityManagement.GetServicePrinicpalNameByAppIdAsync(_appId);
        }

        public Task<bool> IsInAuthorizedGroup()
        {
            return _identityManagement.ServicePrinicpalExistsInGroupAsync(_appId, _groupId);
        }
    }
}
