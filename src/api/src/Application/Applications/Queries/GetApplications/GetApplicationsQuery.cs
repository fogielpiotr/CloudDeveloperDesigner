using MediatR;

namespace Application.Applications.Queries.GetApplications
{

    public class GetApplicationsQuery : IRequest<List<ApplicationDto>>
    {
    }
}
