using Application.Common.Interfaces;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Api.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _groupId;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, IOptions<AzureAdOptions> azureAdOptions)
        {
            _httpContextAccessor = httpContextAccessor;
            _groupId = azureAdOptions.Value.GroupId;
        }

        public Task<string> GetUserName()
        {
            return Task.FromResult(_httpContextAccessor.HttpContext.User.FindFirstValue("name"));
        }

        public Task<bool> IsInAuthorizedGroup()
        {
            return Task.FromResult(_httpContextAccessor.HttpContext.User.Claims.Any(x => x.Type == "groups" && x.Value == _groupId));
        }
    }
}

