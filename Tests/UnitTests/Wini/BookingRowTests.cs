namespace Tests.UnitTests.Wini;

public class BookingRowTests
{

    [Fact]
    public void Create_Booking_Row_With_No_Data()
    {
        var id = new IdValue<int>(1);
        var rowNumber = new BookingRowNumber(1);
        var bookingRow = new BookingRow(id, rowNumber);

        Assert.Equal(default, bookingRow.Account.Value);
        Assert.Equal(default, bookingRow.Account.Subsidiary);
        Assert.Equal(0, bookingRow.Amount.Amount);
        Assert.Equal(default, bookingRow.Amount.Currency.CurrencyRate);
        Assert.Equal(default, bookingRow.Amount.Currency.CurrencyCode.Code);
    }
}