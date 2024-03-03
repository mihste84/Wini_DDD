namespace Domain.Wini.Events;

public class WiniBookingRowActionEvent(BookingRowAction action, BookingRow row, int? bookingId)
: BaseDomainEvent("WiniBookingRowAction")
{
    public readonly BookingRow Row = row;
    public readonly BookingRowAction Action = action;
    public readonly int? BookingId = bookingId;
}