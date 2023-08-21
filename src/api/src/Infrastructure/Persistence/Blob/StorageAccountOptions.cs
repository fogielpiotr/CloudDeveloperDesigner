namespace Infrastructure.Persistance.Blob
{
    public class StorageAccountOptions
    {
        public const string StorageAccount = "StorageAccount";

        public string ConnectionString { get; set; } = string.Empty;
        public string ArmTemplateContainer { get; set; } = string.Empty;
        public string CodeTemplateContainer { get; set; } = string.Empty;
        public string ImageContainer { get; set; } = string.Empty;
    }
}
