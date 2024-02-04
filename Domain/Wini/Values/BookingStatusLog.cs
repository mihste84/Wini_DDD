namespace Domain.Wini.Values;

public record BookingStatusLog
{
    public IdValue<int> BookingId { get; }
    public DateTime Created { get; }
    public WiniStatus Status { get; }
    public User User { get; }
    public BookingStatusLog(IdValue<int> bookingId, WiniStatus status, User user)
    {
        BookingId = bookingId;
        Created = DateTime.UtcNow;
        Status = status;
        User = user;
    }
}