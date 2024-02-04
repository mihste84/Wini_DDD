namespace Services.DatabaseDapper.Models;

public record Booking : BaseModel
{
    public short? Status { get; set; }
    public DateTime? BookingDate { get; set; }
    public string? TextToE1 { get; set; }
    public bool? Reversed { get; set; }
    public short? LedgerType { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? Updated { get; set; }
    public byte[]? RowVersion { get; set; }
}