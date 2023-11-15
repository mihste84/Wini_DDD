namespace Domain.Wini.Aggregates;

public class Booking
{
    public readonly IdValue<int>? Id;
    public readonly Commissioner Commissioner;
    public BookingStatus BookingStatus { get; private set; }
    public BookingDate BookingDate { get; private set; }
    public TextToE1 Description { get; set; }
    public bool IsReversed { get; set; }
    public LedgerType LedgerType { get; set; }
    public List<BookingRow> Rows { get; private set; } = new();
    public List<Comment> Comments { get; private set; } = new();
    public List<RecipientMessage> Messages { get; private set; } = new();
    public List<Attachment> Attachments { get; private set; } = new();
    public readonly DateTime Created;

    public Booking(
        IdValue<int>? id,
        Commissioner commissioner
    )
    {
        Id = id;
        BookingStatus = new BookingStatus(WiniStatus.Saved, DateTime.UtcNow);
        Commissioner = commissioner;
        BookingDate = new BookingDate(DateTime.UtcNow);
        Description = new TextToE1(default);
        IsReversed = false;
        LedgerType = new LedgerType(Ledgers.AA);
        Created = DateTime.UtcNow;
    }

    public Booking(
        IdValue<int>? id,
        BookingStatus status,
        Commissioner commissioner,
        BookingDate bookingDate,
        TextToE1 description,
        bool isReversed,
        LedgerType ledgerType,
        List<BookingRow> rows,
        List<Comment> comments,
        List<RecipientMessage> messages,
        List<Attachment> attachments,
        DateTime created
    )
    {
        Id = id;
        BookingStatus = status;
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

    public void AddNewRow(BookingRowModel row)
    {
        if (BookingStatus.Status != WiniStatus.Saved)
            throw new DomainLogicException("Cannot add new row. Rows can only be added when Booking status is 'Saved'.");

        row.HasAuthorized = false;

        var newRow = MapBookingRowModelToValue(row);

        Rows.Add(newRow);
    }

    public void EditRow(int index, BookingRowModel row)
    {
        if (BookingStatus.Status != WiniStatus.Saved)
            throw new DomainLogicException("Cannot edit row. Rows can only be added when Booking status is 'Saved'.");

        if (Rows.ElementAtOrDefault(index) == null)
            throw new DomainLogicException($"Cannot edit row. Existing row with number {index} could not be found.");

        var editedRow = MapBookingRowModelToValue(row);

        Rows.Insert(index, editedRow);
    }

    public void DeleteRow(int index)
    {
        if (BookingStatus.Status != WiniStatus.Saved)
            throw new DomainLogicException("Cannot delete row. Rows can only be added when Booking status is 'Saved'.");

        if (Rows.ElementAtOrDefault(index) == null)
            throw new DomainLogicException($"Cannot delete row. Existing row with number {index} could not be found.");

        Rows.RemoveAt(index);
    }

    private BookingRow MapBookingRowModelToValue(BookingRowModel row)
    => new BookingRow(
        row.Id.HasValue ? new IdValue<int>(row.Id.Value) : default,
        Id,
        new BusinessUnit(row.BusinessUnit),
        new Account(row.Account, row.Subsidiary),
        new Subledger(row.Subledger, row.SubledgerType),
        new CostObject(1, row.CostObject1, row.CostObjectType1),
        new CostObject(2, row.CostObject2, row.CostObjectType2),
        new CostObject(3, row.CostObject3, row.CostObjectType3),
        new CostObject(4, row.CostObject4, row.CostObjectType4),
        new Remark(row.Remark),
        new Authorizer(row.Authorizer, row.HasAuthorized),
        new Money(row.Amount, row.CurrencyCode, row.ExchangeRate)
    );
}