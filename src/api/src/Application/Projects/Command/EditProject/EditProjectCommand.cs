using MediatR;

namespace Application.Projects.Command.EditProject
{
    public class EditProjectCommand : IRequest
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public string Diagram { get; init; }
        public List<string> Environments { get; init; }
        public string ResourceGroup { get; init; }
        public string Location { get; init; }
        public IDictionary<string, string> MandatoryTags { get; init; }
    }
}
