using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Api.Messaging
{
    [Authorize]
    public class DeploymentStatusHub : Hub {}
}
