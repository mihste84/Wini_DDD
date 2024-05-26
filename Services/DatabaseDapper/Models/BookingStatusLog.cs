namespace Services.DatabaseDapper.Models;

public record BookingStatusLog : BaseModel
{
    public int? BookingId { get; set; }
    public short? Status { get; set; }
}
