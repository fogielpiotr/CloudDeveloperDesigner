using Domain;
using Domain.Deployments;
using Domain.Interfaces;
using Infrastructure.Options;
using Infrastructure.Persistance.Blob;
using Microsoft.Extensions.Options;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Infrastructure.ResourceCreators
{
    public class AzureDevOpsRepositoryCreator : IRemoteCodeRepositoryCreator
    {
        private readonly IBlobManager _blobManager;
        private readonly StorageAccountOptions _storageAccountOptions;
        private readonly AzureDevOpsOptions _azureDevOpsOptions;

        public AzureDevOpsRepositoryCreator(IBlobManager blobManager, IOptions<StorageAccountOptions> storageAccountOptions, IOptions<AzureDevOpsOptions> azuredevOpsOptions)
        {
            _blobManager = blobManager;
            _storageAccountOptions = storageAccountOptions.Value;
            _azureDevOpsOptions = azuredevOpsOptions.Value;
        }

        public async Task<CloudCreatorResponse<Guid>> CreateRemoteCodeRepositoryAsync(RepositoryDeployment repositoryDeployment, CancellationToken ct)
        {
            try
            {
                var dictionaryName = Guid.NewGuid().ToString();
                await CreateCodeRepositoryInDirectoryAsync(repositoryDeployment.SourceZipFileName, dictionaryName, ct);
                await ModifySettingsFile(repositoryDeployment.SettingsFilesNames, repositoryDeployment.SettingJson, dictionaryName);
                var repo = await DeployCodeToGitRepositoryAsync(repositoryDeployment.Name, dictionaryName, ct);
                Directory.Delete(GetFullPath(dictionaryName), true);

                return new CloudCreatorResponse<Guid>(repo.Id, repo.WebUrl);

            }
            catch (Exception ex)
            {
                return new CloudCreatorResponse<Guid>(ex.Message);
            }
        }

        private async Task CreateCodeRepositoryInDirectoryAsync(string codeTemplatePath, string directoryName, CancellationToken ct)
        {
            var path = GetFullPath(directoryName);
            Directory.CreateDirectory(path);
            await _blobManager.ExtractZipBlobToDirectory(_storageAccountOptions.CodeTemplateContainer, codeTemplatePath, path, ct);
        }

        private async static Task ModifySettingsFile(IEnumerable<string> settingsFileNames, string settingsJson, string inputDirectory)
        {
            foreach (var settingFileName in settingsFileNames)
            {
                var settingsFiles = Directory.GetFiles(GetFullPath(inputDirectory), settingFileName, SearchOption.AllDirectories);
                foreach (var settingFile in settingsFiles)
                {
                    JObject fileJson;
                    using (var file = File.OpenText(settingFile))
                    using (var reader = new JsonTextReader(file))
                    {
                        var existingfileJson = (JObject)JToken.ReadFrom(reader);
                        var addedContent = JObject.Parse(settingsJson);
                        existingfileJson.Merge(addedContent, new JsonMergeSettings
                        {
                            MergeArrayHandling = MergeArrayHandling.Union
                        });
                        fileJson = existingfileJson;
                    };

                    using var sw = new StreamWriter(settingFile, append: false);
                    await sw.WriteAsync(JsonConvert.SerializeObject(fileJson, Formatting.Indented));
                }
            }
        }

        private async Task<GitRepository> DeployCodeToGitRepositoryAsync(string appName, string inputDirectory, CancellationToken ct)
        {
            var DevOpsUri = new Uri(_azureDevOpsOptions.Uri);
            var connection = new VssConnection(DevOpsUri, new VssBasicCredential(string.Empty, _azureDevOpsOptions.PAT));
            var projectClient = connection.GetClient<ProjectHttpClient>();
            var project = await projectClient.GetProject(_azureDevOpsOptions.ProjectName);
            var gitClient = connection.GetClient<GitHttpClient>();

            var repo = await gitClient.CreateRepositoryAsync(new GitRepository() { Name = appName, ProjectReference = project }, cancellationToken: ct);

            var folderName = GetFullPath(inputDirectory);
            string[] entries = Directory.GetFileSystemEntries(Path.Combine(folderName), "*", SearchOption.AllDirectories);
            var changes = new List<GitChange>(entries.Length);
            foreach (var entry in entries)
            {
                if (Directory.Exists(entry)) continue;

                string toBeSearched = $"{ folderName }{ Path.DirectorySeparatorChar}";
                string path = entry[(entry.LastIndexOf(toBeSearched) + toBeSearched.Length)..];
                if (path.Contains(Path.DirectorySeparatorChar))
                {
                    path = path.Replace(Path.DirectorySeparatorChar, '/');
                    path = '/' + path;
                }

                changes.Add(new GitChange()
                {
                    ChangeType = VersionControlChangeType.Add,
                    Item = new GitItem() { Path = path, GitObjectType = GitObjectType.Blob, IsFolder = false },
                    NewContent = new ItemContent()
                    {
                        Content = File.ReadAllText(entry),
                        ContentType = ItemContentType.RawText,
                    },
                });

            }
            var commit = new GitCommitRef()
            {
                Comment = "initial commit",
                Changes = changes
            };

            await gitClient.CreatePushAsync(new GitPush()
            {
                RefUpdates = new GitRefUpdate[] { new GitRefUpdate { Name = "refs/heads/develop", OldObjectId = "0000000000000000000000000000000000000000" } },
                Commits = new GitCommitRef[] { commit },
            }, repo.Id, cancellationToken: ct);

            return repo;
        }

        private static string GetFullPath(string directoryName)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "bin", directoryName);
        }
    }
}
