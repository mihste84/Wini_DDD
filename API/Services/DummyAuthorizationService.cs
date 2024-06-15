namespace API.Services;

public class DummyAuthorizationService : Domain.Common.Interfaces.IAuthorizationService
{
    public bool IsAdmin() => true;
    public bool IsRead() => true;
    public bool IsWrite() => true;
    public bool IsAccountingUser() => false;
    public bool IsControlActuary() => false;
    public bool IsSpecificActuary() => false;
    public bool IsBookingAuthorizationNeeded() => true;
    public bool IsUserInRole(string userId, string roleName) => true;
}