namespace Domain.Interfaces;

public interface IAuthenticationService
{
    string GetUserId();
    bool IsAuthenticated();
}