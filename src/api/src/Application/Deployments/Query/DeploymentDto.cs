using Application.Common.Mappings;
using AutoMapper;
using Domain.Deployments;

namespace Application.Deployments.Query
{
    public record DeploymentDto : IMap
    {
        public Guid Id { get; init; }
        public string Creator { get; init; }
        public Guid ProjectId { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public IEnumerable<string> Environments { get; init; }
        public bool CodeDeployment { get; init; }
        public DeploymentStatus Status { get; init; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Deployment, DeploymentDto>()
                .ForMember(dest => dest.Environments,
                           opt => opt.MapFrom((source, dest) =>
                           {
                               if (source.EnvironmentDeployments == null || !source.EnvironmentDeployments.Any())
                               {
                                   return Enumerable.Empty<string>();
                               }

                               return source.EnvironmentDeployments.Select(x => x.Environment);
                           }))
               .ForMember(dest => dest.CodeDeployment,
                          opt => opt.MapFrom((source, dest) =>
                           {
                               if (source.CodeDeployments == null || !source.CodeDeployments.Any())
                               {
                                   return false;
                               }
                               return true;
                           }));
        }
    }
}
