using Application.Common.Interfaces;
using Domain.Deployments;
using Domain.Events;
using Domain.Interfaces;
using MediatR;

namespace Application.Deployments.Command.CreateDeployment
{
    public class CreateDeploymentCommandHandler : IRequestHandler<CreateDeploymentCommand, Unit>
    {
        private readonly IMessageBroker _messageBroker;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDeploymentRepository _deploymentRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IResourceRepository _resourceRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly IClock _clock;

        public CreateDeploymentCommandHandler(
            ICurrentUserService currentUserService,
            IApplicationRepository applicationRepository,
            IDeploymentRepository deploymentRepository,
            IResourceRepository resourceRepository,
            IMessageBroker messageBroker,
            ITemplateRepository templateRepository, 
            IClock clock)
        {
            _currentUserService = currentUserService;
            _applicationRepository = applicationRepository;
            _deploymentRepository = deploymentRepository;
            _resourceRepository = resourceRepository;
            _messageBroker = messageBroker;
            _templateRepository = templateRepository;
            _clock = clock;
        }

        public async Task<Unit> Handle(CreateDeploymentCommand request, CancellationToken cancellationToken)
        {
            var deployment = new Deployment(request.DeploymentId, await _currentUserService.GetUserName(), _clock.CurrentDate(), request.ProjectId);

            if (request.CodeRepositoryDeployments.Any())
            {
                await AddCodeDeployments(request, deployment, cancellationToken);
            }

            if (request.EnvironmentDeployments.Any())
            {
                await AddEnvironmentDeployments(request, deployment, cancellationToken);
            }
            deployment.QueueDeployment();

            await _deploymentRepository.AddAsync(deployment, cancellationToken);

            await _messageBroker.SendMessage(new DeploymentQueued(deployment.Id), cancellationToken);

            return Unit.Value;
        }

        private async Task AddEnvironmentDeployments(CreateDeploymentCommand request, Deployment deployment, CancellationToken cancellationToken)
        {
            var envariomentsDeployments = new List<EnvironmentDeployment>(request.EnvironmentDeployments.Count());
            var availableResources = await _resourceRepository.GetResourcesAsync(cancellationToken);
            var resourceTemplateDictionary = new Dictionary<Guid, string>();
            foreach (var env in request.EnvironmentDeployments)
            {
                var envDeployment = new EnvironmentDeployment(
                        env.Environment,
                        env.ResourceGroup);

                var deploymentObjects = new List<ResourceDeployment>();
                foreach (var resource in env.ResourceDeployments)
                {
                    var databaseResource = availableResources.FirstOrDefault(x => resource.ResourceId == x.Id);
                    var template = string.Empty;
                    if (resourceTemplateDictionary.ContainsKey(resource.ResourceId))
                    {
                        template = resourceTemplateDictionary[resource.ResourceId];
                    }
                    else
                    {
                        template = await _templateRepository.GetTemplateAsync(databaseResource.TemplateFileName, cancellationToken);
                        resourceTemplateDictionary.Add(resource.ResourceId, template);
                    }

                    var deploymentObject = new ResourceDeployment(
                        resource.ResourceId,
                        databaseResource.Name,
                        resource.Name,
                        resource.SecretName,
                        databaseResource.TemplateFileName,
                        template,
                        databaseResource.IsSecretProvider,
                        resource.Parameters);

                    deploymentObjects.Add(deploymentObject);
                }

                envDeployment.AddResources(deploymentObjects);


                var applicationIdentitiesDeploymnets = new List<ApplicationIdentityDeployment>();
                foreach (var application in env.ApplicationIdentityDeployments)
                {
                    var app = new ApplicationIdentityDeployment(application.Name, application.ApplicationType);
                    app.AddAuthorizedApps(application.AuthorizedApps);
                    app.AddRedirectUris(application.RedirectUris);
                    app.AddSecretsNames(application.ClientSecretName, application.ClientIdSecretName);
                    
                    applicationIdentitiesDeploymnets.Add(app);
                }

                envDeployment.AddApplicationIdentities(applicationIdentitiesDeploymnets);
                envariomentsDeployments.Add(envDeployment);
            }

            deployment.AddEnvarionmentDeployments(envariomentsDeployments);
        }

        private async Task AddCodeDeployments(CreateDeploymentCommand request, Deployment deployment, CancellationToken cancellationToken)
        {
            var availableApplications = await _applicationRepository.GetApplicationsAsync(cancellationToken);
            var codeDepoyments = new List<CodeDeployment>();
            foreach (var codeDepolyment in request.CodeRepositoryDeployments)
            {
                var application = availableApplications.First(x => x.Id == codeDepolyment.AppId);
                var repositoryDeployment = new RepositoryDeployment(codeDepolyment.RepositoryName,
                                                                    codeDepolyment.SettingsJson,
                                                                    application.SettingsFiles,
                                                                    application.BlobName);
                var buildDeployment = new BuildDeployment(codeDepolyment.RepositoryName,
                                                          application.BuildFile);

                var codeDeployment = new CodeDeployment();
                codeDeployment.AddBuildDeployment(buildDeployment);
                codeDeployment.AddRepositoryDeployment(repositoryDeployment);
                                                                                                   
                codeDepoyments.Add(codeDeployment);
            }

            deployment.AddCodeDeployments(codeDepoyments);
        }
    }
}
