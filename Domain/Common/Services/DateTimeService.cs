namespace Domain.Common.Services;

public class DateTimeService : IDateTimeService
{
    public DateTime GetDateTimeUtc() => DateTime.UtcNow;
}