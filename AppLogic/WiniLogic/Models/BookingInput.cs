namespace AppLogic.WiniLogic.Models;

public class BookingInput
{
    public BookingRowModel[] Rows { get; set; } = [];
    public DateOnly? BookingDate { get; set; }
    public string? TextToE1 { get; set; }
    public bool? IsReversed { get; set; }
    public Ledgers? LedgerType { get; set; }
}