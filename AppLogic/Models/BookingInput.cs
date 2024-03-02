namespace AppLogic.Models;

public class BookingInput
{
    public BookingRowModel[] Rows { get; set; } = Array.Empty<BookingRowModel>();
    public DateTime? BookingDate { get; set; }
    public string? TextToE1 { get; set; }
    public bool? IsReversed { get; set; }
    public Ledgers? LedgerType { get; set; }
}