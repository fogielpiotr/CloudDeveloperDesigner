using Domain.Applications;

namespace Domain.Deployments
{
    public class ApplicationIdentityDeployment : DeploymentObject<ApplicationIdentityIdentifier>
    {
        public ApplicationIdentityDeployment(string name, ApplicationType type)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type;
        }

        public string ClientSecretName { get; private set; }
        public string ClientIdSecretName { get; private set; }
        public ApplicationType Type { get; init; }
        public IEnumerable<string> AuthorizedApps { get; private set; }
        public IEnumerable<string> RedirectUris { get; private set; }

        public void AddAuthorizedApps(IEnumerable<string> authrorizedApps)
        {
            AuthorizedApps = authrorizedApps;
            
            if (AuthorizedApps == null)
            {
                AuthorizedApps = new List<string>();
            }

            if (AuthorizedApps.Any() && Type == ApplicationType.Web && AuthorizedApps.Any())
            {
                throw new ArgumentException("Cannot add authorized apps for Web Application");
            }
        }

        public void AddRedirectUris(IEnumerable<string> redirectUris)
        {
            RedirectUris = redirectUris;
            
            if (RedirectUris == null)
            {
                RedirectUris = new List<string>();
            }
            
            if (RedirectUris.Any() && Type == ApplicationType.Server)
            {
                throw new ArgumentException("Cannot add redirectsUris for Server Application");
            }

        }
        public void AddSecretsNames(string clientSecretName, string clientIdSecretName)
        {

            if (string.IsNullOrEmpty(clientIdSecretName))
            {
                throw new ArgumentNullException(nameof(clientIdSecretName));
            }

            if (Type == ApplicationType.Web && !string.IsNullOrEmpty(clientSecretName))
            {
                throw new ArgumentException("Cannot add Secret Name for Web Application");
            }
            if (Type == ApplicationType.Server && string.IsNullOrEmpty(clientSecretName))
            {
                throw new ArgumentNullException(nameof(clientSecretName));
            }

            ClientSecretName = clientSecretName;
            ClientIdSecretName = clientIdSecretName;
        }
    }

    public class ApplicationIdentityIdentifier
    {
        public ApplicationIdentityIdentifier(string objectId, string applicationId)
        {
            ObjectId = objectId;
            ApplicationId = applicationId;
        }

        public string ObjectId { get; init; }
        public string ApplicationId { get; init; }
    }
}
