using MediatR;

namespace Application.Resources.Queries.GetResource
{
    public class GetResourcesQuery : IRequest<List<ResourcesDto>>
    {
    }
}
