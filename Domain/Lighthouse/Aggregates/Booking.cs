namespace Domain.Lighthouse.Aggregates;

public class Booking
{
    public readonly IdValue<int>? BookingId;
    public readonly Commissioner Commissioner;
    public BookingStatus Status { get; }
    public BookingHeader Header { get; private set; }
    public List<BaseDomainEvent> DomainEvents { get; } = [];


}