using Application.Common.Mappings;
using AutoMapper;
using Domain.Resources;
using Domain.Templates;

namespace Application.Resources.Queries.GetResource
{
    public class ResourcesDto : IMap
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string IconFile { get; set; }
        public Template Template { get; set; }
        public ResourceNaming ResourceNaming { get; set; }
        public bool IsSecretProvider { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Resource, ResourcesDto>();
        }
    }
}
