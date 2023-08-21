namespace Domain.Interfaces
{
    public interface IResourceManagerFactory
    {
        IResourceManager GetManager(string resourceType);
    }
}
