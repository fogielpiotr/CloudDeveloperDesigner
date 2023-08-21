using Domain.Templates;

namespace Domain.Interfaces
{
    public interface ITemplateRepository
    {
        Task<IEnumerable<Template>> GetTemplatesAsync(CancellationToken ct);
        Task<string> GetTemplateAsync(string blobName, CancellationToken ct);
        Task<Template> GetParsedTemplateAsync(string blobName, CancellationToken ct);
    }
}
