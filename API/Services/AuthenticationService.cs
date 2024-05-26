namespace API.Services;

public class AuthenticationService(IHttpContextAccessor _httpContextAccessor) : IAuthenticationService
{
    public bool IsAuthenticated() => _httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated == true;

    public string GetUserId()
    {
        var userId = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "Not logged in";
        var index = userId.IndexOf('#');

        return index >= 0 ? userId[(index + 1)..] : userId;
    }
}