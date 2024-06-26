using Nager.Date;

namespace Domain.Common.Values;

public readonly record struct BookClosingDay
{
    public readonly PeriodType MonthType;
    public readonly bool IsTodayBookClosingDay;
    public readonly int? Day;
    internal static readonly int[] _querteryMonths = [4, 7, 10];

    public BookClosingDay(DateOnly date, IEnumerable<DateOnly> bankHolidayDates)
    {
        var bookClosingDays = GetBookClosingDates(date, bankHolidayDates);
        IsTodayBookClosingDay = IsBookClosingDay(date, bookClosingDays);

        if (IsTodayBookClosingDay)
        {
            var temp = bookClosingDays.FirstOrDefault(_ => _.Date.Month == date.Month && _.Date.Year == date.Year && _.Date.Day == date.Day);

            Day = temp.Day;
        }

        MonthType = GetMonthType(date, Day);
    }

    private static bool IsWeekendDay(DateOnly checkDate)
    {
        return checkDate.DayOfWeek == DayOfWeek.Saturday || checkDate.DayOfWeek == DayOfWeek.Sunday;
    }

    private static bool IsBankHoliday(DateOnly checkDate, IEnumerable<DateOnly> bankHolidays)
    {
        if (bankHolidays == null)
        {
            throw new ArgumentNullException(nameof(bankHolidays), "Bankholidays collection cannot be null");
        }

        return bankHolidays.Any(d => d.Day == checkDate.Day && d.Month == checkDate.Month && d.Year == checkDate.Year);
    }

    //Check for swedish holidays with third party library which automatically calculates holidays
    private static bool IsSwedishHoliday(DateOnly checkDate)
    {
        return DateSystem.IsPublicHoliday(checkDate.ToDateTime(default), CountryCode.SE);
    }

    //Calculate bookclosing days in current month based on which month it is and if any day of month is weekend/bankholiday/holiday
    private static IEnumerable<(DateOnly Date, int Day)> GetBookClosingDates(DateOnly checkDate, IEnumerable<DateOnly> bankHolidays)
    {
        var dayCount = 0;
        var days = new List<(DateOnly Date, int Day)>();
        var bookClosingMonth = CorrectBookClosingMonth(checkDate); //Current month or next month

        //Iterate through all days in month and check if current day is a non working day. Only working days count as bookclosing days.
        for (var i = 1; i < DateTime.DaysInMonth(bookClosingMonth.Year, bookClosingMonth.Month); i++)
        {
            var date = new DateOnly(bookClosingMonth.Year, bookClosingMonth.Month, i);
            if (!CheckIfNonWorkingDay(date, bankHolidays))
            {
                days.Add((date, ++dayCount));
            }
        }

        //Merge days of current month with negative days of previous month. Take only the first 17 items in list.
        return days.Union(GetNegativeBookClosingDates(bookClosingMonth, bankHolidays)).OrderBy(_ => _.Day).Take(18);
    }

    //Get next month if day in current month is after 23. Used for calculation of bookclosing days.
    private static DateOnly CorrectBookClosingMonth(DateOnly date)
    {
        if (date.Day > 23)
        {
            if (date.Month == 12)
            {
                return new DateOnly(date.Year + 1, 1, 1); //Next year if month is December
            }

            return new DateOnly(date.Year, date.Month + 1, 1);
        }

        return date;
    }

    //Get book closing day -3 to -1. Negative bookclosing days belong to previous month
    private static IEnumerable<(DateOnly Date, int Day)> GetNegativeBookClosingDates(
        DateOnly checkDate,
        IEnumerable<DateOnly> bankHolidays)
    {
        var year = (checkDate.Month == 1) ? checkDate.Year - 1 : checkDate.Year;
        var month = (checkDate.Month == 1) ? 12 : checkDate.Month - 1;
        var lastDay = DateTime.DaysInMonth(year, month);
        var days = new List<(DateOnly Date, int Day)>();
        var dayCount = 0;
        for (var i = 0; i > -7; i--)
        {
            var date = new DateOnly(year, month, lastDay).AddDays(i);

            if (!CheckIfNonWorkingDay(date, bankHolidays))
            {
                days.Add((date, --dayCount));
            }
        }

        return days.Take(3);
    }

    private static bool CheckIfNonWorkingDay(DateOnly checkDate, IEnumerable<DateOnly> bankHolidays)
    => IsWeekendDay(checkDate) || IsSwedishHoliday(checkDate) || IsBankHoliday(checkDate, bankHolidays);

    //Check if passed date is a book closing day.
    private static bool IsBookClosingDay(DateOnly checkDate, IEnumerable<(DateOnly Date, int Day)> bookClosingDays)
    => bookClosingDays.Any(_ => _.Date.Year == checkDate.Year && _.Date.Month == checkDate.Month && _.Date.Day == checkDate.Day);

    //Get month type based on month number. Month 1 is Yearly bookclosing. Month 4, 7, and 10 are quarterly. Rest are regular monthly bookclosing.
    private static PeriodType GetMonthType(DateOnly checkDate, int? bookClosingDay = null)
    {
        var checkMonth = GetCheckMonth(checkDate.Month, bookClosingDay);
        if (checkMonth == 1)
        {
            return PeriodType.Yearly;
        }

        if (Array.Exists(_querteryMonths, _ => _ == checkMonth))
        {
            return PeriodType.Quarterly;
        }

        return PeriodType.Monthly;
    }

    private static int GetCheckMonth(int currentMonth, int? bookClosingDay = null)
    {
        if (bookClosingDay.HasValue && bookClosingDay < 1)
        {
            if (currentMonth == 12)
            {
                return 1;
            }

            return currentMonth + 1;
        }

        return currentMonth;
    }
}