namespace Infrastructure.Options
{
    public class AzureDevOpsOptions
    {
        public const string AzureDevOps = "AzureDevOps";
        public string PAT { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string Uri { get; set; } = string.Empty; 
    }
}
