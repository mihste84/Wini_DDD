namespace DatabaseEf.Models;

public class RecipientMessage
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public string Value { get; set; } = null!;

    public string Recipient { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateTime Created { get; set; }

    public virtual Booking Booking { get; set; } = null!;
}
