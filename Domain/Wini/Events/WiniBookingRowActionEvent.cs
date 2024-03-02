namespace Domain.Wini.Events;

public class WiniBookingRowActionEvent : BaseDomainEvent
{
    public readonly BookingRow Row;
    public readonly BookingRowAction Action;
    public readonly int? BookingId;
    public WiniBookingRowActionEvent(BookingRowAction action, BookingRow row, int? bookingId) : base("WiniBookingRowAction")
    {
        Row = row;
        Action = action;
        BookingId = bookingId;
    }
}