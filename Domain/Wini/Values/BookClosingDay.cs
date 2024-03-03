using Nager.Date;

namespace Domain.Wini.Values;

record BookClosingDay
{
    public readonly PeriodType MonthType;
    public readonly bool IsTodayBookClosingDay;
    public readonly int? Day;
    internal static readonly int[] _querteryMonths = [4, 7, 10];

    public BookClosingDay(DateTime date, IEnumerable<DateTime> bankHolidayDates)
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

    private static bool IsWeekendDay(DateTime checkDate)
    {
        return checkDate.DayOfWeek == DayOfWeek.Saturday || checkDate.DayOfWeek == DayOfWeek.Sunday;
    }

    private static bool IsBankHoliday(DateTime checkDate, IEnumerable<DateTime> bankHolidays)
    {
        if (bankHolidays == null)
        {
            throw new ArgumentNullException(nameof(bankHolidays), "Bankholidays collection cannot be null");
        }

        return bankHolidays.Any(d => d.Day == checkDate.Day && d.Month == checkDate.Month && d.Year == checkDate.Year);
    }

    //Check for swedish holidays with third party library which automatically calculates holidays
    private static bool IsSwedishHoliday(DateTime checkDate)
    {
        return DateSystem.IsPublicHoliday(checkDate, CountryCode.SE);
    }

    //Calculate bookclosing days in current month based on which month it is and if any day of month is weekend/bankholiday/holiday
    private static IEnumerable<(DateTime Date, int Day)> GetBookClosingDates(DateTime checkDate, IEnumerable<DateTime> bankHolidays)
    {
        var dayCount = 0;
        var days = new List<(DateTime Date, int Day)>();
        var bookClosingMonth = CorrectBookClosingMonth(checkDate); //Current month or next month

        //Iterate through all days in month and check if current day is a non working day. Only working days count as bookclosing days.
        for (var i = 1; i < DateTime.DaysInMonth(bookClosingMonth.Year, bookClosingMonth.Month); i++)
        {
            var date = new DateTime(bookClosingMonth.Year, bookClosingMonth.Month, i);
            if (!CheckIfNonWorkingDay(date, bankHolidays))
            {
                days.Add((date, ++dayCount));
            }
        }

        //Merge days of current month with negative days of previous month. Take only the first 17 items in list.
        return days.Union(GetNegativeBookClosingDates(bookClosingMonth, bankHolidays)).OrderBy(_ => _.Day).Take(18);
    }

    //Get next month if day in current month is after 23. Used for calculation of bookclosing days. 
    private static DateTime CorrectBookClosingMonth(DateTime date)
    {
        if (date.Day > 23)
        {
            if (date.Month == 12)
            {
                return new DateTime(date.Year + 1, 1, 1); //Next year if month is December
            }

            return new DateTime(date.Year, date.Month + 1, 1);
        }

        return date;
    }

    //Get book closing day -3 to -1. Negative bookclosing days belong to previous month
    private static IEnumerable<(DateTime Date, int Day)> GetNegativeBookClosingDates(
        DateTime checkDate,
        IEnumerable<DateTime> bankHolidays)
    {
        var year = (checkDate.Month == 1) ? checkDate.Year - 1 : checkDate.Year;
        var month = (checkDate.Month == 1) ? 12 : checkDate.Month - 1;
        var lastDay = DateTime.DaysInMonth(year, month);
        var days = new List<(DateTime Date, int Day)>();
        var dayCount = 0;
        for (var i = 0; i > -7; i--)
        {
            var date = new DateTime(year, month, lastDay).AddDays(i);

            if (!CheckIfNonWorkingDay(date, bankHolidays))
            {
                days.Add((date, --dayCount));
            }
        }

        return days.Take(3);
    }

    private static bool CheckIfNonWorkingDay(DateTime checkDate, IEnumerable<DateTime> bankHolidays)
    => IsWeekendDay(checkDate) || IsSwedishHoliday(checkDate) || IsBankHoliday(checkDate, bankHolidays);

    //Check if passed date is a book closing day.
    private static bool IsBookClosingDay(DateTime checkDate, IEnumerable<(DateTime Date, int Day)> bookClosingDays)
    => bookClosingDays.Any(_ => _.Date.Year == checkDate.Year && _.Date.Month == checkDate.Month && _.Date.Day == checkDate.Day);

    //Get month type based on month number. Month 1 is Yearly bookclosing. Month 4, 7, and 10 are quarterly. Rest are regular monthly bookclosing.
    private static PeriodType GetMonthType(DateTime checkDate, int? bookClosingDay = null)
    {
        var checkMonth = GetCheckMonth(checkDate.Month, bookClosingDay);
        if (checkMonth == 1)
        {
            return PeriodType.Yearly;
        }

        if (_querteryMonths.Any(_ => _ == checkMonth))
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