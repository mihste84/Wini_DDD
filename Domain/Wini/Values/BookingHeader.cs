namespace Domain.Wini.Values;

public record BookingHeader
{
    public BookingDate BookingDate { get; }
    public TextToE1 TextToE1 { get; }
    public bool IsReversed { get; }
    public LedgerType LedgerType { get; }

    public BookingHeader()
    {
        BookingDate = new BookingDate(DateTime.UtcNow);
        TextToE1 = new TextToE1(default);
        IsReversed = false;
        LedgerType = new LedgerType(Ledgers.AA);
    }

    public BookingHeader(DateTime bookingDate, string? textToE1, bool isReversed, Ledgers ledgerType)
    {
        BookingDate = new BookingDate(bookingDate);
        TextToE1 = new TextToE1(textToE1);
        IsReversed = isReversed;
        LedgerType = new LedgerType(ledgerType);
    }

    public BookingHeader(BookingDate bookingDate, TextToE1 textToE1, bool isReversed, LedgerType ledgerType)
    {
        BookingDate = bookingDate;
        TextToE1 = textToE1;
        IsReversed = isReversed;
        LedgerType = ledgerType;
    }
}