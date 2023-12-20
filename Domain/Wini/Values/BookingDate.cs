namespace Domain.Wini.Values;

public record BookingDate
{
    public readonly DateTime Date;

    public BookingDate(DateTime date)
    {
        Date = date.Date;
    }

    public bool TryCheckIfDateBookClosingDay(IEnumerable<DateTime> bankHolidays, out int? bookClosingDay)
    {
        var bookClosing = new BookClosingDay(Date, bankHolidays);
        bookClosingDay = bookClosing.Day;
        return bookClosing.IsTodayBookClosingDay;
    }

    public static bool TryCheckIfDateBookClosingDay(DateTime dateToCheck, IEnumerable<DateTime> bankHolidays, out int? bookClosingDay)
    {
        var bookClosing = new BookClosingDay(dateToCheck, bankHolidays);
        bookClosingDay = bookClosing.Day;
        return bookClosing.IsTodayBookClosingDay;
    }

    public bool IsPeriodClosed(DateTime today)
    {
        var startOfCurrentPeriod = new DateTime(today.Year, today.Month, 1);
        return startOfCurrentPeriod.Date <= Date;
    }

    public static bool IsPeriodEnd(int currentPeriod, int? bookClosingDay, DeviatingPeriodSettings deviatingPeriodSettings)
    {
        var periodEnd = GetPeriodEnd(currentPeriod, deviatingPeriodSettings);
        return periodEnd >= bookClosingDay;
    }

    static int GetPeriodEnd(int period, DeviatingPeriodSettings deviatingPeriodSettings)
    {
        return deviatingPeriodSettings.DeviatingPeriods.Contains(period)
            ? deviatingPeriodSettings.DeviatingPeriodEnd
            : deviatingPeriodSettings.NormalPeriodEnd;
    }
}