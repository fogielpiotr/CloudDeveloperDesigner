using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;

namespace Application.Configuration.Query.GetConfigurations
{
    public class GetConfigurationsQueryHandler : IRequestHandler<GetConfigurationsQuery, Domain.Configuration>
    {
        private readonly IConfigurationRepository _configurationRepository;

        public GetConfigurationsQueryHandler(IConfigurationRepository configurationRepository)
        {
            _configurationRepository = configurationRepository;
        }

        public Task<Domain.Configuration> Handle(GetConfigurationsQuery request, CancellationToken cancellationToken)
        {
            return _configurationRepository.GetConfigurationAsync(cancellationToken);
        }
    }
}
