namespace Services.DatabaseDapper.Models;

public record Comment : BaseModel
{
    public int? BookingId { get; set; }
    public string? Value { get; set; }
}
