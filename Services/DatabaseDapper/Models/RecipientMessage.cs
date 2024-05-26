namespace Services.DatabaseDapper.Models;

public record RecipientMessage : BaseModel
{
    public int? BookingId { get; set; }
    public string? Value { get; set; }
    public string? Recipient { get; set; }
}
