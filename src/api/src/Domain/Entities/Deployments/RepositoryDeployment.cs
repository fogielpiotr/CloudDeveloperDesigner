namespace Domain.Deployments
{
    public class RepositoryDeployment : DeploymentObject<Guid>
    {
        protected RepositoryDeployment(){}

        public RepositoryDeployment(string name, string settingJson, IEnumerable<string> settingsFilesNames, string sourceZipFileName)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            SettingJson = settingJson;
            SettingsFilesNames = settingsFilesNames;
            SourceZipFileName = sourceZipFileName ?? throw new ArgumentNullException(nameof(sourceZipFileName));
        }

        public string SettingJson { get; init; }
        public IEnumerable<string> SettingsFilesNames { get; init; }
        public string SourceZipFileName { get; init; }
    }
}
