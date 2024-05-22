namespace API.Endpoints;

public static class AppUserEndpoints
{
    public static IResult GetAppUser(Domain.Common.Interfaces.IAuthorizationService authorizationService)
    {
        var rights = new AppUserRights(
            authorizationService.IsAdmin(),
            authorizationService.IsRead(),
            authorizationService.IsWrite(),
            authorizationService.IsAccountingUser(),
            authorizationService.IsControlActuary(),
            authorizationService.IsSpecificActuary(),
            authorizationService.IsBookingAuthorizationNeeded()
        );

        return Results.Ok(rights);
    }
}