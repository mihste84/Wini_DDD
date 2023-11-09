namespace Domain.Interfaces;

public interface IAuthorizationService
{
    bool IsAdmin();
    bool IsRead();
    bool IsWrite();
    bool IsAccountingUser();
    bool IsControlActuary();
    bool IsSpecificActuary();
}