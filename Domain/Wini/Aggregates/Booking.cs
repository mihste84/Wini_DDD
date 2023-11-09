namespace Domain.Wini.Aggregates;

public class Booking
{
    public readonly IdValue<int> Id;
    public BookingStatus Status { get; }
    public Commissioner Commissioner { get; }
    public BookingDate BookingDate { get; }
    public Description Description { get; }
    public bool IsReversed { get; }
    public LedgerType LedgerType { get; }
    public BookingRow[] Rows { get; } = Array.Empty<BookingRow>();
    public Comment[] Comments { get; } = Array.Empty<Comment>();
    public RecipientMessage[] Messages { get; } = Array.Empty<RecipientMessage>();
    public Attachment[] Attachments { get; } = Array.Empty<Attachment>();
    public readonly DateTime Created;

    public Booking(
        IdValue<int> id,
        Commissioner commissioner
    )
    {
        Id = id;
        Status = new BookingStatus(WiniStatus.Saved, DateTime.UtcNow);
        Commissioner = commissioner;
        BookingDate = new BookingDate(DateTime.UtcNow);
        Description = new Description("");
        IsReversed = false;
        LedgerType = new LedgerType(Ledgers.AA);
        Created = DateTime.UtcNow;
    }

    public Booking(
        IdValue<int> id,
        BookingStatus status,
        Commissioner commissioner,
        BookingDate bookingDate,
        Description description,
        bool isReversed,
        LedgerType ledgerType,
        BookingRow[] rows,
        Comment[] comments,
        RecipientMessage[] messages,
        Attachment[] attachments,
        DateTime created
    )
    {
        Id = id;
        Status = status;
        Commissioner = commissioner;
        BookingDate = bookingDate;
        Description = description;
        IsReversed = isReversed;
        LedgerType = ledgerType;
        Rows = rows;
        Comments = comments;
        Messages = messages;
        Attachments = attachments;
        Created = created;
    }
}