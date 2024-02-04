namespace API.Services;

public class TestAuthenticationService : IAuthenticationService
{
    public string GetUserId() => "test_user@mail.com";
    public bool IsAuthenticated() => true;
}
