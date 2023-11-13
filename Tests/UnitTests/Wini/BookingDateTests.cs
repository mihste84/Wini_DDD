namespace Tests.UnitTests.Wini;

public class BookingDateTests
{
    private readonly IReadOnlyList<DateTime> _bankHolidays = new List<DateTime>{
        new DateTime(2023, 1, 1),
        new DateTime(2023, 1, 6),
        new DateTime(2023, 4, 7),
        new DateTime(2023, 4, 10),
        new DateTime(2023, 5, 1),
        new DateTime(2023, 5, 18),
        new DateTime(2023, 6, 6),
        new DateTime(2023, 12, 25),
        new DateTime(2023, 12, 26)
    };

    [Fact]
    public void Create_Booking_Date()
    {
        var today = new DateTime(2023, 1, 23, 23, 59, 59);
        var bookingDate = new BookingDate(today);
        Assert.Equal(today.Date, bookingDate.Date.Date);
        Assert.Equal(0, bookingDate.Date.Hour);
        Assert.Equal(0, bookingDate.Date.Minute);
        Assert.Equal(0, bookingDate.Date.Second);
        Assert.Equal(0, bookingDate.Date.Millisecond);
        Assert.Equal(0, bookingDate.Date.Microsecond);
    }

    [Theory]
    [InlineData(2022, 12, 30, true, -1)]
    [InlineData(2022, 12, 31, false, default)]
    [InlineData(2023, 1, 1, false, default)]
    [InlineData(2023, 1, 2, true, 1)]
    [InlineData(2023, 1, 6, false, default)]
    [InlineData(2023, 1, 9, true, 5)]
    [InlineData(2023, 1, 10, true, 6)]
    [InlineData(2023, 1, 31, true, -1)]
    [InlineData(2023, 1, 30, true, -2)]
    [InlineData(2023, 4, 6, true, 4)]
    [InlineData(2023, 4, 7, false, default)]
    [InlineData(2023, 4, 10, false, default)]
    [InlineData(2023, 4, 11, true, 5)]
    [InlineData(2023, 4, 17, true, 9)]
    [InlineData(2023, 4, 28, true, -1)]
    [InlineData(2023, 5, 1, false, default)]
    [InlineData(2023, 5, 2, true, 1)]
    [InlineData(2023, 5, 18, false, default)]
    [InlineData(2023, 5, 19, true, 13)]
    [InlineData(2023, 6, 5, true, 3)]
    [InlineData(2023, 6, 6, false, default)]
    [InlineData(2023, 6, 7, true, 4)]
    [InlineData(2023, 12, 28, true, -2)]
    [InlineData(2023, 12, 29, true, -1)]
    public void Check_Is_Date_Book_Closing_Day(int year, int month, int day, bool isBookClosingDay, int? expectedBookClosingDay)
    {
        var dateToCheck = new DateTime(year, month, day);
        var bookingDate = new BookingDate(dateToCheck);

        var isBookClosingDayResult = bookingDate.TryCheckIfDateBookClosingDay(_bankHolidays, out var bookClosingDay);

        Assert.Equal(isBookClosingDay, isBookClosingDayResult);
        Assert.Equal(expectedBookClosingDay, bookClosingDay);
    }

    [Theory]
    [InlineData(5, 3, true)]
    [InlineData(5, 4, true)]
    [InlineData(5, 5, false)]
    public void Check_If_Period_End_With_Normal_Period_And_Deviations(int currentPeriod, int currentBookClosingDay, bool expectedResult)
    {
        var deviatingPeriodSettings = new DeviatingPeriodSettings();
        var isPeriodEnd = BookingDate.IsPeriodEnd(currentPeriod, currentBookClosingDay, deviatingPeriodSettings);

        Assert.Equal(expectedResult, isPeriodEnd);
    }

    [Theory]
    [InlineData(5, 3, true)]
    [InlineData(8, 4, true)]
    [InlineData(8, 6, true)]
    [InlineData(8, 10, true)]
    [InlineData(8, 11, false)]
    [InlineData(8, 15, false)]
    [InlineData(9, 1, true)]
    [InlineData(9, 5, false)]
    public void Check_If_Period_End_With_Deviating_Periods(int currentPeriod, int currentBookClosingDay, bool expectedResult)
    {
        var deviatingPeriodSettings = new DeviatingPeriodSettings
        {
            DeviatingPeriods = new List<int> { 8 }
        };
        var isPeriodEnd = BookingDate.IsPeriodEnd(currentPeriod, currentBookClosingDay, deviatingPeriodSettings);

        Assert.Equal(expectedResult, isPeriodEnd);
    }

    [Theory]
    [InlineData(2023, 1, 1, true)]
    [InlineData(2023, 2, 1, true)]
    [InlineData(2023, 2, 28, true)]
    [InlineData(2023, 3, 1, true)]
    [InlineData(2023, 3, 5, true)]
    [InlineData(2023, 3, 31, true)]
    [InlineData(2023, 4, 1, false)]
    [InlineData(2023, 5, 1, false)]

    public void Check_Is_Period_Closed(int year, int month, int day, bool expectedResult)
    {
        var dateToCheck = new DateTime(year, month, day);
        var bookingDatePeriod = new DateTime(2023, 3, 24);
        var bookingDate = new BookingDate(bookingDatePeriod);

        var isPeriodClosed = bookingDate.IsPeriodClosed(dateToCheck);

        Assert.Equal(isPeriodClosed, expectedResult);
    }
}