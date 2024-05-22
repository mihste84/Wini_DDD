namespace Domain.Common.Extensions;

public static class DateTimeExtensions
{
    public static bool CompareWithoutMilliseconds(this DateTime input, DateTime comparer)
    => new DateTime(input.Year, input.Month, input.Day, input.Hour, input.Minute, input.Second, 0, input.Kind) ==
       new DateTime(comparer.Year, comparer.Month, comparer.Day, comparer.Hour, comparer.Minute, comparer.Second, 0, comparer.Kind);
}