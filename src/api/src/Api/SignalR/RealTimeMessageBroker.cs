using Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Api.Messaging
{
    public class RealTimeMessageBroker : IRealTimeMessageBroker
    {
        private readonly IHubContext<DeploymentStatusHub> _deploymentHubContext;

        public RealTimeMessageBroker(IHubContext<DeploymentStatusHub> deploymentHubContext)
        {
            _deploymentHubContext = deploymentHubContext;
        }

        public async Task SendMessage<T>(string method, T message)
        {
            await _deploymentHubContext.Clients.All.SendAsync(method, message);
        }
    }
}
