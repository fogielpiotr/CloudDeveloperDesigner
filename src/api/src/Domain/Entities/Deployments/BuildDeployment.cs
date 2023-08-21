namespace Domain.Deployments
{
    public class BuildDeployment : DeploymentObject<int>
    {
        protected BuildDeployment() { }
        public BuildDeployment(string name, string buildDefinitionFileName)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            BuildDefinitionFileName = buildDefinitionFileName ?? throw new ArgumentNullException(nameof(buildDefinitionFileName));
        }

        public string BuildDefinitionFileName { get; private set; }
    }
}
