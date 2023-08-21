using Application.Common.Mappings;
using AutoMapper;
using Domain.Projects;

namespace Application.Projects.Queries;

public record ProjectDto: IMap
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public DateTimeOffset Created { get; init; }
    public DateTimeOffset? Updated { get; init; }
    public string CreatedBy { get; init; }
    public string UpdatedBy { get; init; }
    public string Description { get; init; }
    public string Diagram { get; init; }
    public List<string> Environments { get; init; }
    public IDictionary<string,string> MandatoryTags { get; init; }
    public string ResourceGroup { get; init; }
    public string Location { get; init; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Project, ProjectDto>();
    }
}
