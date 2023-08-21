namespace Domain.Deployments
{
    public class EnvironmentDeployment
    {
        protected EnvironmentDeployment() { }
        public EnvironmentDeployment(string environment, ResourceGroupDeployment resourceGroup)
        {
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
            ResourceGroup = resourceGroup;
        }

        public string Environment { get; init; }
        public ResourceGroupDeployment ResourceGroup { get; init; }
        public IEnumerable<ResourceDeployment> ResourceDeployments { get; private set; }
        public IEnumerable<ApplicationIdentityDeployment> ApplicationIdentityDeployments { get; private set; }

        public void AddResources(IEnumerable<ResourceDeployment> resources)
        {
            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }
            if (resources.Count(x => x.IsSercretProvider) > 1)
            {
                throw new ArgumentException("more that one sercret provider");
            }

            ResourceDeployments = resources;
        }
        public void AddApplicationIdentities(IEnumerable<ApplicationIdentityDeployment> applicationIdentityDeployments)
        {
            ApplicationIdentityDeployments = applicationIdentityDeployments ?? throw new ArgumentNullException(nameof(applicationIdentityDeployments));
        }
    }
}
