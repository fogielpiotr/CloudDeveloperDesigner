using Application.Common.Mappings;
using AutoMapper;
using Domain.Applications;

namespace Application.Applications.Queries.GetApplications
{
    public class ApplicationDto : IMap
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string IconFile { get; set; }
        public ApplicationType Type { get; set; }
        
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Domain.Applications.Application, ApplicationDto>();
        }
    }
}
