using Domain.Interfaces;

namespace Infrastructure.ResourceManagement
{
    internal class ResourceManagerFactory : IResourceManagerFactory
    {
        private readonly IEnumerable<IResourceManager> _resourceManagers;

        public ResourceManagerFactory(IEnumerable<IResourceManager> resourceManagers)
        {
            _resourceManagers = resourceManagers;
        }

        public IResourceManager GetManager(string resourceName)
        {
            return _resourceManagers.FirstOrDefault(x => x.Name == resourceName);
        }
    }
}
