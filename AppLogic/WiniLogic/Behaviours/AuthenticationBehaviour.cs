namespace AppLogic.Wini.LogicBehaviours;

public class AuthenticationBehaviour<TRequest, TResponse>(IAuthenticationService _authService)
: IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    => !_authService.IsAuthenticated() ? throw new UnauthorizedAccessException() : await next();
}