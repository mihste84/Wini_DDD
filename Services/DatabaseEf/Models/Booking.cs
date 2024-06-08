namespace DatabaseEf.Models;

public class Booking
{
    public int Id { get; set; }

    public short Status { get; set; }

    public DateTime BookingDate { get; set; }

    public string? TextToE1 { get; set; }

    public bool Reversed { get; set; }

    public short LedgerType { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime Created { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public DateTime Updated { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<Attachment> Attachments { get; set; } = [];

    public virtual ICollection<BookingRow> BookingRows { get; set; } = [];

    public virtual ICollection<BookingStatusLog> BookingStatusLogs { get; set; } = [];

    public virtual ICollection<Comment> Comments { get; set; } = [];

    public virtual ICollection<RecipientMessage> RecipientMessages { get; set; } = [];
}
