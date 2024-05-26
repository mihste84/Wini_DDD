namespace API.Services;

public class TestAuthenticationService : IAuthenticationService
{
    public const string UserId = "test_user@mail.com";
    public string GetUserId() => UserId;
    public bool IsAuthenticated() => true;
}
