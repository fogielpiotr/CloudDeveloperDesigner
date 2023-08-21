namespace Domain.Deployments
{
    public class Deployment : Entity
    {
        protected Deployment() { }
        public Deployment(Guid id, string creator, DateTimeOffset date, Guid projectId)
        {
            Id = id;
            Creator = creator ?? throw new ArgumentNullException(nameof(creator));
            CreatedAt = date;
            ProjectId = projectId;
        }

        public string Creator { get; init; }
        public Guid ProjectId { get; init; }
        public DateTimeOffset CreatedAt { get; init; }     
        public DeploymentStatus Status { get; private set; }
        public IEnumerable<CodeDeployment> CodeDeployments { get; private set; }
        public IEnumerable<EnvironmentDeployment> EnvironmentDeployments { get; set; }

        public void AddCodeDeployments(IEnumerable<CodeDeployment> codeDeployments)
        {
            CodeDeployments = codeDeployments;
            if (CodeDeployments == null)
            {
                CodeDeployments = new List<CodeDeployment>();
            }
        }

        public void AddEnvarionmentDeployments(IEnumerable<EnvironmentDeployment> environmentDeployments)
        {
            EnvironmentDeployments = environmentDeployments;
            if (EnvironmentDeployments == null)
            {
                EnvironmentDeployments = new List<EnvironmentDeployment>();
            }
        }

        public void QueueDeployment()
        {
            Status = DeploymentStatus.DeploymentQueued;
        }

        public void FinishDeployment()
        {
            Status = DeploymentStatus.DeploymentFinished;
        }

        public void FailureDeployment()
        {
            Status = DeploymentStatus.DeploymentFailed;
        }
    }
}
