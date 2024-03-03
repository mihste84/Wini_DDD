namespace API.Services;

public class AuthenticationService(IHttpContextAccessor _httpContextAccessor) : IAuthenticationService
{
    public string GetUserId() => _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "Not logged in";
    public bool IsAuthenticated() => _httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated == true;
}