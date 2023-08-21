namespace Domain.Deployments
{
    public class ResourceDeployment: DeploymentObject<string>
    {
        protected ResourceDeployment() { }
        public ResourceDeployment(Guid resourceId, string resourceName, string name, string secretName, string templateBlobName, string template, bool isSecretProvider, IDictionary<string, ParameterValue> parameters)
        {
            ResourceId = resourceId;
            ResourceName = resourceName ?? throw new ArgumentNullException(nameof(resourceName));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            SecretName = secretName;
            TemplateBlobName = templateBlobName ?? throw new ArgumentNullException(nameof(templateBlobName));
            Template = template ?? throw new ArgumentNullException(nameof(template));
            IsSercretProvider = isSecretProvider;
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }

        public Guid ResourceId { get; init; }
        public string ResourceName { get; init; }
        public string SecretName { get; init; }
        public string TemplateBlobName { get; init; }
        public string Template { get; init; }
        public bool IsSercretProvider { get; init; }
        public IDictionary<string, ParameterValue> Parameters { get; init; }
    }
}
