namespace Services.DatabaseCommon.Models;

public class Attachment
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public int Size { get; set; }

    public string ContentType { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Path { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateTime Created { get; set; }

    public virtual Booking Booking { get; set; } = null!;
}
