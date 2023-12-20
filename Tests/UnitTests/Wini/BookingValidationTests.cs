namespace Tests.UnitTests.Wini;

public class BookingValidationTests
{


    private BookingValidationService GetBookingValidationService(
        string currentUser = "MIHSTE",
        bool isAdmin = false,
        bool isAuthNeeded = true
    )
    {
        var authorizationService = new Mock<IAuthorizationService>();
        authorizationService.Setup(_ => _.IsAdmin()).Returns(isAdmin);
        authorizationService.Setup(_ => _.IsBookingAuthorizationNeeded()).Returns(isAuthNeeded);
        var authorizerValidationService = new Mock<IAuthorizerValidationService>();
        authorizerValidationService.Setup(_ => _.CanAuthorizeBookingRows(It.IsAny<IEnumerable<BookingRow>>()))
            .ReturnsAsync((true, Array.Empty<ValidationError>()));
        var bookingPeriodValidationService = new Mock<IBookingPeriodValidationService>();
        bookingPeriodValidationService.Setup(_ => _.ValidateAsync(It.IsAny<Booking>()))
            .ReturnsAsync((true, Array.Empty<ValidationError>()));
        var accountingValidationService = new Mock<IAccountingValidationService>();
        accountingValidationService.Setup(_ => _.ValidateAsync(It.IsAny<IEnumerable<AccountingValidationInputModel>>()))
            .ReturnsAsync((true, Array.Empty<ValidationError>()));

        return new(
            authorizationService.Object,
            authorizerValidationService.Object,
            bookingPeriodValidationService.Object,
            accountingValidationService.Object,
            new TestWiniUnitOfWork()
        );
    }
}