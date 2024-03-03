namespace Domain.Wini.Events;

public class WiniBookingRowDeleteEvent(int rowNumber, int bookingId) : BaseDomainEvent("WiniBookingRowDelete")
{
    public readonly int RowNumber = rowNumber;
    public readonly int BookingId = bookingId;
}