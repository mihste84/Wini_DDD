namespace API.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticationService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetUserId() => _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "Not logged in";

    public bool IsAuthenticated() => _httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated == true;
}