namespace Domain.Deployments
{
    public class DeploymentLog
    {
        public DeploymentLog(Guid id, DateTimeOffset date, Guid deploymentId, string message, string url, string environment, bool codeDeployment, string name, string error, DeploymentStatus status)
        {
            Id = id;
            Date = date;
            DeploymentId = deploymentId;
            Message = message;
            Url = url;
            Environment = environment;
            CodeDeployment = codeDeployment;
            Name = name;
            Status = status;
            Error = error;
        }

        public Guid Id { get; init; }
        public DateTimeOffset Date { get; init; }
        public Guid DeploymentId { get; init; }
        public string Message { get; init; }
        public string Url { get; init; }
        public string Environment { get; init; }
        public string Error { get; init; }
        public bool CodeDeployment { get; init; }
        public string Name { get; init; }
        public DeploymentStatus Status { get; init; }
    }
}
