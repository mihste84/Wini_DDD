namespace Domain.Wini.Events;

public class WiniStatusEvent(BookingStatus status) : BaseDomainEvent("WiniStatusChange")
{
    public readonly BookingStatus Status = status;
}