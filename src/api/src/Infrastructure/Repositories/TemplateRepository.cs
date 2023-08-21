using Domain.Interfaces;
using Domain.Templates;
using Infrastructure.Persistance.Blob;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Infrastructure.Services
{
    public class TemplateRepository : ITemplateRepository
    {
        private readonly IBlobManager _blobManager;
        private readonly string _armTemplateContainer;

        public TemplateRepository(IBlobManager blobManager, IOptions<StorageAccountOptions> storageAccountOptions)
        {
            _blobManager = blobManager;
            _armTemplateContainer = storageAccountOptions.Value.ArmTemplateContainer;
        }

        public async Task<IEnumerable<Template>> GetTemplatesAsync(CancellationToken ct)
        {
            var result = new List<Template>();
            foreach (var blob in await _blobManager.GetFilesContentAsync(_armTemplateContainer, ct))
            {
                var armTemplate = JsonConvert.DeserializeObject<Template>(blob);
                if (armTemplate == null)
                {
                    throw new KeyNotFoundException($"File is not ArmTemplate");
                }
                result.Add(armTemplate);
            }

            return result;
        }

        public Task<string> GetTemplateAsync(string blobName, CancellationToken ct)
        {
            return _blobManager.GetFileContentAsync(_armTemplateContainer, blobName, ct);
        }

        public async Task<Template> GetParsedTemplateAsync(string blobName, CancellationToken ct)
        {
            var armTemplate  = await _blobManager.GetFileContentAsync(_armTemplateContainer, blobName, ct);
            var parsedArmTemplate = JsonConvert.DeserializeObject<Template>(armTemplate);

            return parsedArmTemplate;
        }
    }
}
