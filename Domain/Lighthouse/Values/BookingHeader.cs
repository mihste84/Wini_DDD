
namespace Domain.Lighthouse.Values;

public record BookingHeader
{
    public BookingDate BookingDate { get; }
    public bool IsReversed { get; }
    public InsuranceType InsuranceType { get; }

    public BookingHeader()
    {
        BookingDate = new BookingDate(DateTime.UtcNow);
        IsReversed = false;
        // InsuranceType = new LedgerType(Ledgers.AA);
    }

    public BookingHeader(DateOnly bookingDate, bool isReversed, InsuranceType type)
    {
        BookingDate = new BookingDate(bookingDate);
        IsReversed = isReversed;
        // InsuranceType = new LedgerType(ledgerType);
    }

    public BookingHeader(BookingDate bookingDate, bool isReversed, InsuranceType ledgerType)
    {
        BookingDate = bookingDate;
        IsReversed = isReversed;
        InsuranceType = ledgerType;
    }
}