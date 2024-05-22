namespace Domain.Common.Interfaces;

public interface IAuthenticationService
{
    string GetUserId();
    bool IsAuthenticated();
}