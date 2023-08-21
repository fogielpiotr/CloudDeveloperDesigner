namespace Domain.Deployments
{
    public class ResourceGroupDeployment : DeploymentObject<string>
    {
        public string Location { get; set; }
        public Dictionary<string, string> Tags { get; set; }
    }
}
