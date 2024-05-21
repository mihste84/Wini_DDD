namespace Domain.Interfaces;

public interface IAuthorizationService
{
    bool IsAdmin();
    bool IsRead();
    bool IsWrite();
    bool IsAccountingUser();
    bool IsControlActuary();
    bool IsSpecificActuary();
    bool IsBookingAuthorizationNeeded();
    bool IsUserInRole(string userId, string roleName);
}