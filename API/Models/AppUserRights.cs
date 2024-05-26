namespace API.Models;
public record AppUserRights(
    bool IsAdmin,
    bool IsRead,
    bool IsWrite,
    bool IsAccountingUser,
    bool IsControlActuary,
    bool IsSpecificActuary,
    bool IsBookingAuthorizationNeeded
);