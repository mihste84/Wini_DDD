namespace Domain.Wini.Events;

public class WiniBookingRowDeleteEvent : BaseDomainEvent
{
    public readonly int RowNumber;
    public readonly int BookingId;
    public WiniBookingRowDeleteEvent(int rowNumber, int bookingId) : base("WiniBookingRowDelete")
    {
        RowNumber = rowNumber;
        BookingId = bookingId;
    }
}