namespace Domain.Wini.Events;

public class WiniStatusEvent : BaseDomainEvent
{
    public readonly Booking Booking;
    public readonly WiniStatus Status;
    public WiniStatusEvent(WiniStatus status, Booking booking) : base("WiniStatusChange")
    {
        Booking = booking;
        Status = status;
    }
}