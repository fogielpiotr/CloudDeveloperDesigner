namespace Domain.Applications
{
    public class Application
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string BlobName { get; set; }
        public string IconFile { get; set; }
        public string BuildFile { get; set; }
        public IEnumerable<string> SettingsFiles { get; set; }
        public ApplicationType Type { get; set; }
    }

    public enum ApplicationType
    {
        Web = 1,
        Server
    }
}
