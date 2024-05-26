namespace Services.DatabaseDapper.Models;

public record Attachment : BaseModel
{
    public int? BookingId { get; set; }
    public int? Size { get; set; }
    public string? ContentType { get; set; }
    public string? Name { get; set; }
    public string? Path { get; set; }
}
