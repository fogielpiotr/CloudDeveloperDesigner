namespace Domain.Deployments
{
    public class CodeDeployment
    {
        public RepositoryDeployment RepositoryDeployment { get; private set; }
        public BuildDeployment BuildDeployment { get; private set; }

        public void AddRepositoryDeployment(RepositoryDeployment repositoryDeployment)
        {
            RepositoryDeployment = repositoryDeployment ?? throw new ArgumentNullException(nameof(repositoryDeployment));
        }

        public void AddBuildDeployment(BuildDeployment buildDeployment)
        {
            BuildDeployment = buildDeployment ?? throw new ArgumentNullException(nameof(buildDeployment));
        }
    }
}
