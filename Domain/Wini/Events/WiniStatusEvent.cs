namespace Domain.Wini.Events;

public class WiniStatusEvent : BaseDomainEvent
{
    public readonly BookingStatus Status;
    public WiniStatusEvent(BookingStatus status) : base("WiniStatusChange")
    {
        Status = status;
    }
}