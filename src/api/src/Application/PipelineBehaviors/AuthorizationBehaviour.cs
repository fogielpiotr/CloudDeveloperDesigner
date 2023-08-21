using Application.Common.Interfaces;
using MediatR;

namespace Application.PipelineBehaviors
{
    public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ICurrentUserService _currentUserService;

        public AuthorizationBehaviour(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (!await _currentUserService.IsInAuthorizedGroup())
            {
                throw new UnauthorizedAccessException(
                    $"Authorization failed for: {await _currentUserService.GetUserName()}. Please contact the Administrator.");
            }
            
            return await next();
        }
    }
}
