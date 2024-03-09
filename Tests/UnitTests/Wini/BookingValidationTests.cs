namespace Tests.UnitTests.Wini;

public class BookingValidationTests
{
    private readonly IEnumerable<Company> _companies = CommonTestValues.GetCompanies();

    [Fact]
    public async Task Validate_Booking_Async()
    {
        var rows = new List<BookingRow> {
            new(
                1,
                new BusinessUnit("100KKTOT"),
                new Account("10500"),
                new Subledger("1234", "A"),
                new CostObject(1, "1111", "A"),
                new CostObject(2, "2222", "B"),
                new CostObject(3,"3333", "C"),
                new CostObject(4, "4444", "D"),
                new Remark("Test"),
                new Authorizer("XMIHST", false),
                new Money(100, "SEK", 0)
            ),
            new(
                2,
                new BusinessUnit("100KKTOT"),
                new Account("24500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark(),
                new Authorizer(),
                new Money(-100, "SEK", 0)
            )
        };
        var booking = CommonTestValues.GetBooking(rows, "MIHSTE", WiniStatus.Saved);
        var validationService = GetBookingValidationService();

        var res = await validationService.ValidateAsync(booking, _companies);

        Assert.NotNull(res);
        Assert.True(res.IsValid);
        Assert.Null(res.Errors);
    }

    [Fact]
    public async Task Validate_Empty_Booking_Async()
    {
        var booking = CommonTestValues.GetNewEmptyBooking();
        var validationService = GetBookingValidationService();

        var res = await validationService.ValidateAsync(booking, _companies);

        Assert.NotNull(res);
        Assert.False(res.IsValid);
        Assert.NotNull(res.Errors);
        Assert.Equal(2, res.Errors.Count());
        Assert.Contains(res.Errors, _ => _.PropertyName == "Header.TextToE1.Text");
        Assert.Contains(res.Errors, _ => _.Message == "'TextToE1' must not be empty.");
        Assert.Contains(res.Errors, _ => _.PropertyName == "Rows");
        Assert.Contains(res.Errors, _ => _.Message == "Booking must contain rows.");
    }

    [Fact]
    public async Task Validate_Booking_With_Empty_Rows_Async()
    {
        var rows = new List<BookingRow> {
            new(
                1,
                new BusinessUnit(),
                new Account(),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark(),
                new Authorizer(),
                new Money()
            ),
            new(
                2,
                new BusinessUnit(),
                new Account(),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark(),
                new Authorizer(),
                new Money()
            ),
        };
        var booking = CommonTestValues.GetBooking(rows);
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(booking.Commissioner.UserId!);

        var now = DateTime.UtcNow;
        booking.EditBookingHeader(
            new BookingHeaderModel(DateOnly.FromDateTime(now), "", true, Ledgers.AA), authenticationService.Object
        );

        var validationService = GetBookingValidationService();

        var res = await validationService.ValidateAsync(booking, _companies);

        Assert.NotNull(res);
        Assert.False(res.IsValid);
        Assert.NotNull(res.Errors);
        Assert.Equal(7, res.Errors.Count());
        Assert.Contains(res.Errors, _ => _.PropertyName == "Header.TextToE1.Text" && _.Message == "'TextToE1' must not be empty.");
        Assert.Contains(res.Errors, _ => _.PropertyName == "#1" && _.Message == "'Account' must not be empty.");
        Assert.Contains(res.Errors, _ => _.PropertyName == "#2" && _.Message == "'Account' must not be empty.");
        Assert.Contains(res.Errors, _ => _.PropertyName == "#1" && _.Message == "'Amount' must not be equal to '0'.");
        Assert.Contains(res.Errors, _ => _.PropertyName == "#2" && _.Message == "'Amount' must not be equal to '0'.");
        Assert.Contains(res.Errors, _ => _.PropertyName == "#1" && _.Message == "'Currency Code' must not be empty.");
        Assert.Contains(res.Errors, _ => _.PropertyName == "#2" && _.Message == "'Currency Code' must not be empty.");
    }

    [Fact]
    public async Task Validate_Booking_Authorized_While_Status_Saved_Async()
    {
        var rows = new List<BookingRow> {
            new(
                1,
                new BusinessUnit("100KKTOT"),
                new Account("10500"),
                new Subledger("1234", "A"),
                new CostObject(1, "1111", "A"),
                new CostObject(2, "2222", "B"),
                new CostObject(3,"3333", "C"),
                new CostObject(4, "4444", "D"),
                new Remark("Test"),
                new Authorizer("XMIHST", true),
                new Money(100, "SEK", 0)
            ),
            new(
                2,
                new BusinessUnit("100KKTOT"),
                new Account("24500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark(),
                new Authorizer(),
                new Money(-100, "SEK", 0)
            )
        };
        var booking = CommonTestValues.GetBooking(rows, "MIHSTE", WiniStatus.Saved);
        var validationService = GetBookingValidationService();

        var res = await validationService.ValidateAsync(booking, _companies);

        Assert.NotNull(res);
        Assert.False(res.IsValid);
        Assert.NotNull(res.Errors);
        Assert.Single(res.Errors);
        Assert.Contains(res.Errors, _ => _.PropertyName == "#1" && _.Message == "Booking cannot be authorized while status is Saved.");
    }

    [Fact]
    public async Task Validate_Booking_Not_Same_Company_On_All_Rows_Async()
    {
        var rows = new List<BookingRow> {
            new(
                1,
                new BusinessUnit("100KKTOT"),
                new Account("10500"),
                new Subledger("1234", "A"),
                new CostObject(1, "1111", "A"),
                new CostObject(2, "2222", "B"),
                new CostObject(3,"3333", "C"),
                new CostObject(4, "4444", "D"),
                new Remark("Test"),
                new Authorizer("XMIHST", true),
                new Money(100, "SEK", 0)
            ),
            new(
                2,
                new BusinessUnit("200KKTOT"),
                new Account("24500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark(),
                new Authorizer(),
                new Money(-100, "SEK", 0)
            )
        };
        var booking = CommonTestValues.GetBooking(rows, "MIHSTE", WiniStatus.Saved);
        var validationService = GetBookingValidationService();

        var res = await validationService.ValidateAsync(booking, _companies);

        Assert.NotNull(res);
        Assert.False(res.IsValid);
        Assert.NotNull(res.Errors);
        Assert.Single(res.Errors);
        Assert.Contains(
            res.Errors,
            _ => _.PropertyName == "Company" && _.Message == "All rows dont contain same company code. Only one company code can be used for each booking."
        );
    }

    [Fact]
    public async Task Validate_Booking_Not_Base_Currency_GP_Ledger_Async()
    {
        var rows = new List<BookingRow> {
            new(
                1,
                new BusinessUnit("100KKTOT"),
                new Account("10500"),
                new Subledger("1234", "A"),
                new CostObject(1, "1111", "A"),
                new CostObject(2, "2222", "B"),
                new CostObject(3,"3333", "C"),
                new CostObject(4, "4444", "D"),
                new Remark("Test"),
                new Authorizer("XMIHST", false),
                new Money(100, "NOK", 1)
            ),
            new(
                2,
                new BusinessUnit("100KKTOT"),
                new Account("24500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark(),
                new Authorizer(),
                new Money(-100, "NOK", 1)
            )
        };
        var booking = CommonTestValues.GetBooking(rows, "MIHSTE", WiniStatus.Saved);
        var validationService = GetBookingValidationService();

        var res = await validationService.ValidateAsync(booking, _companies);

        Assert.NotNull(res);
        Assert.False(res.IsValid);
        Assert.NotNull(res.Errors);
        Assert.Equal(2, res.Errors.Count());
        Assert.Contains(res.Errors, _ => _.PropertyName == "#1" && _.Message == "Cannot use GP ledger as currency 'NOK' is not base currency for company '100'.");
        Assert.Contains(res.Errors, _ => _.PropertyName == "#2" && _.Message == "Cannot use GP ledger as currency 'NOK' is not base currency for company '100'.");
    }

    [Fact]
    public async Task Validate_Booking_No_Exchange_Rate_For_Foreign_Currency_AA_Ledger_Async()
    {
        var rows = new List<BookingRow> {
            new(
                1,
                new BusinessUnit("100KKTOT"),
                new Account("10500"),
                new Subledger("1234", "A"),
                new CostObject(1, "1111", "A"),
                new CostObject(2, "2222", "B"),
                new CostObject(3,"3333", "C"),
                new CostObject(4, "4444", "D"),
                new Remark("Test"),
                new Authorizer("XMIHST", false),
                new Money(100, "NOK", 0)
            ),
            new(
                2,
                new BusinessUnit("100KKTOT"),
                new Account("24500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark(),
                new Authorizer(),
                new Money(-100, "NOK", 0)
            )
        };
        var booking = CommonTestValues.GetBooking(rows, "MIHSTE", WiniStatus.Saved, Ledgers.AA);
        var validationService = GetBookingValidationService();

        var res = await validationService.ValidateAsync(booking, _companies);

        Assert.NotNull(res);
        Assert.False(res.IsValid);
        Assert.NotNull(res.Errors);
        Assert.Equal(2, res.Errors.Count());
        Assert.Contains(res.Errors, _ => _.PropertyName == "#1" && _.Message == "Exchange rate with currency 'NOK' must be set when foreign currency is used.");
        Assert.Contains(res.Errors, _ => _.PropertyName == "#2" && _.Message == "Exchange rate with currency 'NOK' must be set when foreign currency is used.");
    }

    [Fact]
    public async Task Validate_Booking_Different_Exchange_Rates_Async()
    {
        var rows = new List<BookingRow> {
            new(
                1,
                new BusinessUnit("100KKTOT"),
                new Account("10500"),
                new Subledger("1234", "A"),
                new CostObject(1, "1111", "A"),
                new CostObject(2, "2222", "B"),
                new CostObject(3,"3333", "C"),
                new CostObject(4, "4444", "D"),
                new Remark("Test"),
                new Authorizer("XMIHST", true),
                new Money(100, "NOK", 1)
            ),
            new(
                2,
                new BusinessUnit("100KKTOT"),
                new Account("10500"),
                new Subledger("1234", "A"),
                new CostObject(1, "1111", "A"),
                new CostObject(2, "2222", "B"),
                new CostObject(3,"3333", "C"),
                new CostObject(4, "4444", "D"),
                new Remark("Test"),
                new Authorizer("XMIHST", true),
                new Money(-50, "NOK", 1)
            ),
            new(
                3,
                new BusinessUnit("100KKTOT"),
                new Account("24500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark(),
                new Authorizer(),
                new Money(-50, "NOK", 0)
            )
        };
        var booking = CommonTestValues.GetBooking(rows, "MIHSTE", WiniStatus.Saved);
        var validationService = GetBookingValidationService();

        var res = await validationService.ValidateAsync(booking, _companies);

        Assert.NotNull(res);
        Assert.False(res.IsValid);
        Assert.NotNull(res.Errors);
        Assert.Single(res.Errors);
        Assert.Contains(res.Errors, _ => _.PropertyName == "Exchange Rate" && _.Message == "Exchange rates '1, 0' do not balance with currency code 'NOK'.");
    }

    [Fact]
    public async Task Validate_Booking_Balance_Differences_By_Currency_Async()
    {
        var rows = new List<BookingRow> {
            new(
                1,
                new BusinessUnit("100KKTOT"),
                new Account("10500"),
                new Subledger("1234", "A"),
                new CostObject(1, "1111", "A"),
                new CostObject(2, "2222", "B"),
                new CostObject(3,"3333", "C"),
                new CostObject(4, "4444", "D"),
                new Remark("Test"),
                new Authorizer("XMIHST", true),
                new Money(100, "NOK", 1)
            ),
            new(
                2,
                new BusinessUnit("100KKTOT"),
                new Account("10500"),
                new Subledger("1234", "A"),
                new CostObject(1, "1111", "A"),
                new CostObject(2, "2222", "B"),
                new CostObject(3,"3333", "C"),
                new CostObject(4, "4444", "D"),
                new Remark("Test"),
                new Authorizer("XMIHST", true),
                new Money(-50, "NOK", 1)
            ),
            new(
                3,
                new BusinessUnit("100KKTOT"),
                new Account("24500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark(),
                new Authorizer(),
                new Money(-49, "NOK", 1)
            )
        };
        var booking = CommonTestValues.GetBooking(rows, "MIHSTE", WiniStatus.Saved);
        var validationService = GetBookingValidationService();

        var res = await validationService.ValidateAsync(booking, _companies);

        Assert.NotNull(res);
        Assert.False(res.IsValid);
        Assert.NotNull(res.Errors);
        Assert.Single(res.Errors);
        Assert.Contains(res.Errors, _ => _.PropertyName == "Debit & Credit" && _.Message == "Debit and credit must be equal when using currency code 'NOK'. Balance = 1");
    }

    [Fact]
    public async Task Validate_Booking_Invalid_Subledger_CostObject_And_Authorizer_Values_Async()
    {
        var rows = new List<BookingRow> {
            new(
                1,
                new BusinessUnit("100KKTOT"),
                new Account("12345"),
                new Subledger("", "B"), // Missing value and wrong type 2
                new CostObject(1, "", "X"), //Missing value 1
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark(),
                new Authorizer("XMIHST", false), // Authorizer on credit 1
                new Money(-100, "SEK", 0)
            ),
            new(
                2,
                new BusinessUnit("100KKTOT"),
                new Account("12345"),
                new Subledger("XYZ", ""),
                new CostObject(1, "XYZ", ""),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark(),
                new Authorizer("XMIHST", false),
                new Money(100, "SEK", 0)
            ),
        };
        var booking = CommonTestValues.GetBooking(rows);
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(booking.Commissioner.UserId!);

        var now = DateTime.UtcNow;
        booking.EditBookingHeader(
            new BookingHeaderModel(DateOnly.FromDateTime(now), "Test", true, Ledgers.AA), authenticationService.Object
        );

        var validationService = GetBookingValidationService();

        var res = await validationService.ValidateAsync(booking, _companies);

        Assert.NotNull(res);
        Assert.False(res.IsValid);
        Assert.NotNull(res.Errors);
        Assert.Equal(5, res.Errors.Count());
        Assert.Contains(res.Errors, _ => _.PropertyName == "#1" && _.Message == "Subledger cannot be empty when Subledger Type has a value.");
        Assert.Contains(res.Errors, _ => _.PropertyName == "#1" && _.Message == "Cost Object 1 must be entered if Cost Object Type 1 is to be used.");
        Assert.Contains(res.Errors, _ => _.PropertyName == "#1" && _.Message == "Authorizer cannot be set on credit rows.");
        Assert.Contains(res.Errors, _ => _.PropertyName == "#2" && _.Message == "Subledger Type must be 'A'.");
        Assert.Contains(res.Errors, _ => _.PropertyName == "#2" && _.Message == "Cost Object Type 1 must be entered if Cost Object 1 is used.");
    }

    private static BookingValidationService GetBookingValidationService(
        bool isAdmin = false,
        bool isAuthNeeded = true
    )
    {
        var authorizationService = new Mock<IAuthorizationService>();
        authorizationService.Setup(_ => _.IsAdmin()).Returns(isAdmin);
        authorizationService.Setup(_ => _.IsBookingAuthorizationNeeded()).Returns(isAuthNeeded);
        var authorizerValidationService = new Mock<IAuthorizerValidationService>();
        authorizerValidationService.Setup(_ => _.CanAuthorizeBookingRowsAsync(It.IsAny<IEnumerable<BookingRow>>()))
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
            accountingValidationService.Object
        );
    }
}