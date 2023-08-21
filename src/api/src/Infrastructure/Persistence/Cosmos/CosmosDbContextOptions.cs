namespace Infrastructure.Persistance.Cosmos
{
    public class CosmosDbContextOptions
    {
        public const string CosmosDbContext = "CosmosDbContext";

        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string ProjectsContainerName { get; set; } = string.Empty;
        public string ResourcesContainerName { get; set; } = string.Empty;
        public string ApplicationsContainerName { get; set; } = string.Empty;
        public string DeploymentsContainerName { get; set; } = string.Empty;
        public string DeploymentLogsContainerName { get; set; } = string.Empty;
        public string ConfigurationContainerName { get; set;} = string.Empty;
    }
}
