namespace Tests.Utils;

public static class CommonTestValues
{
    public static Company[] GetCompanies() => [
        new Company(new IdValue<int>(1), new CompanyCode("100"), new CompanyName("Test 1"), new Country("SE")),
        new Company(new IdValue<int>(2), new CompanyCode("200"), new CompanyName("Test 2"), new Country("NO")),
        new Company(new IdValue<int>(3), new CompanyCode("300"), new CompanyName("Test 3"), new Country("DK")),
        new Company(new IdValue<int>(3), new CompanyCode("400"), new CompanyName("Test 4"), new Country("FI"))
    ];

    public static Booking GetNewEmptyBooking()
    => new(
        default,
        new Commissioner("MIHSTE")
    );

    public static Booking GetBooking(string commissioner = "XMIHST", WiniStatus status = WiniStatus.Saved)
    => new(
        new IdValue<int>(1),
        new BookingStatus(status, new DateTime(2023, 3, 20, 23, 0, 0), commissioner),
        new Commissioner(commissioner),
        new BookingDate(new DateTime(2023, 3, 22)),
        new TextToE1("Test"),
        true,
        new LedgerType(Ledgers.GP),
        [],
        [],
        [],
        [],
        new DateTime(2023, 3, 20, 23, 0, 0)
    );

    public static Booking GetBooking(List<BookingRow> rows, string commissioner = "XMIHST", WiniStatus status = WiniStatus.Saved, Ledgers ledger = Ledgers.GP)
    => new(
        new IdValue<int>(1),
        new BookingStatus(status, new DateTime(2023, 3, 20, 23, 0, 0), commissioner),
        new Commissioner(commissioner),
        new BookingDate(new DateTime(2023, 3, 22)),
        new TextToE1("Test"),
        true,
        new LedgerType(ledger),
        rows,
        [],
        [],
        [],
        new DateTime(2023, 3, 20, 23, 0, 0)
    );
}