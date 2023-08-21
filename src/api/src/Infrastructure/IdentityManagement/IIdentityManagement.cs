namespace Azure.Infrastructure.IdentityManagement
{
    public interface IIdentityManagement
    {
        Task<string> GetServicePrinicpalNameByAppIdAsync(string identifier);
        Task<bool> ServicePrinicpalExistsInGroupAsync(string spIdentifier, string groupIdentifier);
    }
}
