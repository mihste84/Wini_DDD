namespace Domain.Wini.Values;

public record BookingDate
{
    public readonly DateOnly Date;

    public BookingDate(DateTime date)
    {
        Date = DateOnly.FromDateTime(date);
    }

    public BookingDate(DateOnly date)
    {
        Date = date;
    }

    public bool TryCheckIfDateBookClosingDay(IEnumerable<DateOnly> bankHolidays, out int? bookClosingDay)
    {
        var bookClosing = new BookClosingDay(Date, bankHolidays);
        bookClosingDay = bookClosing.Day;
        return bookClosing.IsTodayBookClosingDay;
    }

    public static bool TryCheckIfDateBookClosingDay(DateOnly dateToCheck, IEnumerable<DateOnly> bankHolidays, out int? bookClosingDay)
    {
        var bookClosing = new BookClosingDay(dateToCheck, bankHolidays);
        bookClosingDay = bookClosing.Day;
        return bookClosing.IsTodayBookClosingDay;
    }

    public bool IsPeriodClosed(DateOnly today)
    {
        var startOfCurrentPeriod = new DateOnly(today.Year, today.Month, 1);
        return startOfCurrentPeriod <= Date;
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