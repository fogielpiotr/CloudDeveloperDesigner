using Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace Application.Resources.Queries.GetResource
{
    public class GetResourcesQueryHandler : IRequestHandler<GetResourcesQuery, List<ResourcesDto>>
    {
        private readonly IMapper _mapper;
        private readonly IResourceRepository _resourceRepository;
        private readonly ITemplateRepository _templateRepository;
        public GetResourcesQueryHandler(IMapper mapper, IResourceRepository azureResourceRepository, ITemplateRepository armTemplateRepository)
        {
            _mapper = mapper;
            _resourceRepository = azureResourceRepository;
            _templateRepository = armTemplateRepository;
        }

        public async Task<List<ResourcesDto>> Handle(GetResourcesQuery request, CancellationToken cancellationToken)
        {
            var azureResource = await _resourceRepository.GetResourcesAsync(cancellationToken);
            var mapped = _mapper.Map<List<ResourcesDto>>(azureResource);
            foreach(var resource in mapped)
            {
                var blobName = azureResource.First(x => x.Id == resource.Id).TemplateFileName;
                resource.Template = await _templateRepository.GetParsedTemplateAsync(blobName, cancellationToken);
            }

            return mapped;
        }
    }
}
