namespace Services.DatabaseDapper.Models;

public record BookingLog : BaseModel
{
    public int? BookingId { get; set; }
    public WiniStatus? Size { get; set; }
    public string? ContentType { get; set; }
    public string? Name { get; set; }
    public string? Path { get; set; }
}
