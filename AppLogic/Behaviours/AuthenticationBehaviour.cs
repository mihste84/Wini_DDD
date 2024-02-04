namespace AppLogic.Behaviours;

public class AuthenticationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IAuthenticationService _authService;

    public AuthenticationBehaviour(IAuthenticationService authService)
    {
        _authService = authService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        return !_authService.IsAuthenticated() ? throw new UnauthorizedAccessException() : await next();
    }
}