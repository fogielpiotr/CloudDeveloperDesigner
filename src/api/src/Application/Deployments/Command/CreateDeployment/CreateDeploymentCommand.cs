using Domain.Applications;
using Domain.Deployments;
using MediatR;

namespace Application.Deployments.Command.CreateDeployment
{
    public record CreateDeploymentCommand : IRequest<Unit>
    {
        public Guid DeploymentId = Guid.NewGuid();
        public Guid ProjectId { get; init; }
        public IEnumerable<EnvironmentDeploymentCommand> EnvironmentDeployments { get; init; }
        public IEnumerable<CodeDeploymentCommand> CodeRepositoryDeployments { get; init; }
    }

    public record EnvironmentDeploymentCommand
    {
        public string Environment { get; init; }
        public ResourceGroupDeployment ResourceGroup { get; init; }
        public IEnumerable<ResourceDeploymentCommand> ResourceDeployments { get; init; }
        public IEnumerable<ApplicationIdentityDeploymentCommand> ApplicationIdentityDeployments { get; init; }
    }

    public record ResourceDeploymentCommand
    {
        public Guid ResourceId { get; init; }
        public string Name { get; init; }
        public string SecretName { get; init; }
        public Dictionary<string, ParameterValue> Parameters { get; init; }
    }

    public record CodeDeploymentCommand
    {
        public Guid AppId { get; init; }
        public string RepositoryName { get; init; }
        public string SettingsJson { get; init; }
    }

    public record ApplicationIdentityDeploymentCommand
    {
        public string Name { get; init; }
        public string ClientSecretName { get; init; }
        public string ClientIdSecretName { get; init; }
        public ApplicationType ApplicationType { get; init; }
        public IEnumerable<string> RedirectUris { get; init; }
        public IEnumerable<string> AuthorizedApps { get; init; }
    }
}
