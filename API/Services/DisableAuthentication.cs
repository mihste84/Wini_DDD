using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace API.Services;

public class DisableAuthenticationPolicyEvaluator : IPolicyEvaluator
{
    public async Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
    {
        var authenticationTicket = new AuthenticationTicket(new ClaimsPrincipal(), new AuthenticationProperties(), JwtBearerDefaults.AuthenticationScheme);
        return await Task.FromResult(AuthenticateResult.Success(authenticationTicket));
    }

    public async Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy, AuthenticateResult authenticationResult, HttpContext context, object? resource)
    {
        return await Task.FromResult(PolicyAuthorizationResult.Success());
    }
}