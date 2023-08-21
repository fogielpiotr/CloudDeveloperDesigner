using Domain.Deployments;

namespace Domain.Events
{
    public class DeploymentEvent
    {
        public DateTimeOffset Date { get; } = DateTimeOffset.UtcNow;
        public Guid DeploymentId { get; init; } 
        public string Message { get; init; }
        public string Error { get; init; }
        public string Name { get; init; }
        public dynamic Id { get; init; }
        public string Url { get; init; }
        public string Environment { get; init; }
        public bool CodeDeployment { get; init; }
        public DeploymentStatus Status { get; init; }
    }
}
