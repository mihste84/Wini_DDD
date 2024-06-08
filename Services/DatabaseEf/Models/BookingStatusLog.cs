namespace DatabaseEf.Models;

public class BookingStatusLog
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public short Status { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime Created { get; set; }

    public virtual Booking Booking { get; set; } = null!;
}
