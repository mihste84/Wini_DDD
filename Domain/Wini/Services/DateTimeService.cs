namespace Domain.Wini.Services;

public class DateTimeService : IDateTimeService
{
    public DateTime GetDateTimeUtc() => DateTime.UtcNow;
}