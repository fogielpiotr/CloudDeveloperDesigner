using Domain;
using Domain.Deployments;
using Domain.Interfaces;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Infrastructure.ResourceCreators
{
    public class AzureDevOpsBuildCreator : IBuildCreator
    {
        private readonly AzureDevOpsOptions _azureDevOpsOptions;
        private readonly BuildHttpClient _buildClient;
        private readonly ProjectHttpClient _projectHttpClient;

        public AzureDevOpsBuildCreator(IOptions<AzureDevOpsOptions> azuredevOpsOptions)
        {
            _azureDevOpsOptions = azuredevOpsOptions.Value;
            var DevOpsUri = new Uri(_azureDevOpsOptions.Uri);
            var connection = new VssConnection(DevOpsUri, new VssBasicCredential(string.Empty, _azureDevOpsOptions.PAT));

            _buildClient = connection.GetClient<BuildHttpClient>();
            _projectHttpClient = connection.GetClient<ProjectHttpClient>();
        }

        public async Task<CloudCreatorResponse<int>> CreateAndQueueBuildForRepository(Domain.Deployments.BuildDeployment buildDeployment, RepositoryDeployment repositoryDeployment, CancellationToken ct)
        {
            try
            {
                var buildDefinition = await CreateBuildDefinition(buildDeployment, repositoryDeployment.CloudIdentifier, ct);
                var url = buildDefinition.Links.Links["web"] as ReferenceLink;
                await QueueBuild(buildDefinition.Id, ct);

                return new CloudCreatorResponse<int>(buildDefinition.Id, url.Href);
            }
            catch (Exception ex)
            {
                return new CloudCreatorResponse<int>(ex.Message);
            }
        }

        private async Task<BuildDefinition> CreateBuildDefinition(Domain.Deployments.BuildDeployment buildDeployment, Guid repositoryId, CancellationToken ct)
        {
            var project = await _projectHttpClient.GetProject(_azureDevOpsOptions.ProjectName);
            
            var process = new YamlProcess()
            {
                YamlFilename = buildDeployment.BuildDefinitionFileName,
            };
            var repository = new BuildRepository
            {
                Id = repositoryId.ToString(),
                Type = "TfsGit"
            };
            var newBuild = new BuildDefinition()
            {
                Name = buildDeployment.Name,
                Process = process,
                Repository = repository,
                Project = new TeamProjectReference
                {
                    Id = project.Id
                },
                Queue = new AgentPoolQueue()
                {
                    Pool = new TaskAgentPoolReference()
                    {
                        IsHosted = true,
                        Name = "Azure Piplines"
                    }
                },
                QueueStatus = DefinitionQueueStatus.Enabled,
            };
            var trigger = new ContinuousIntegrationTrigger();
            trigger.BranchFilters.Add("refs/heads/develop");
            newBuild.Triggers.Add(trigger);
            
            var buildDefinitionResult = await _buildClient.CreateDefinitionAsync(newBuild, cancellationToken: ct);

            return buildDefinitionResult;
        }

        private async Task QueueBuild(int buildDefinitionId, CancellationToken ct)
        {
            var project = await _projectHttpClient.GetProject(_azureDevOpsOptions.ProjectName);
            var build = new Build()
            {
                Project = new TeamProjectReference
                {
                    Id = project.Id
                },
                Definition = new DefinitionReference
                {
                    Id = buildDefinitionId
                },
                SourceBranch = "refs/heads/develop",
            };

            await _buildClient.QueueBuildAsync(build, cancellationToken: ct);
        }
    }
}
