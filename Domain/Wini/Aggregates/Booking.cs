namespace Domain.Wini.Aggregates;

public class Booking
{
    public readonly IdValue<int>? BookingId;
    public readonly Commissioner Commissioner;
    public BookingStatus BookingStatus { get; private set; }
    public BookingDate BookingDate { get; private set; }
    public TextToE1 TextToE1 { get; set; }
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
        BookingId = id;
        BookingStatus = new BookingStatus(WiniStatus.Saved, DateTime.UtcNow);
        Commissioner = commissioner;
        BookingDate = new BookingDate(DateTime.UtcNow);
        TextToE1 = new TextToE1(default);
        IsReversed = false;
        LedgerType = new LedgerType(Ledgers.AA);
        Created = DateTime.UtcNow;
    }

    public Booking(
        IdValue<int>? id,
        BookingStatus status,
        Commissioner commissioner,
        BookingDate bookingDate,
        TextToE1 textToE1,
        bool isReversed,
        LedgerType ledgerType,
        List<BookingRow> rows,
        List<Comment> comments,
        List<RecipientMessage> messages,
        List<Attachment> attachments,
        DateTime created
    )
    {
        BookingId = id;
        BookingStatus = status;
        Commissioner = commissioner;
        BookingDate = bookingDate;
        TextToE1 = textToE1;
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

        var newRow = MapBookingRowModelToValue(row);

        Rows.Add(newRow);
    }

    public void EditRow(BookingRowModel row)
    {
        if (BookingStatus.Status != WiniStatus.Saved)
            throw new DomainLogicException("Cannot edit row. Rows can only be added when Booking status is 'Saved'.");

        var rowToEdit = Rows.SingleOrDefault(_ => _.RowId?.Value == row.Id);

        if (rowToEdit == null)
            throw new DomainLogicException($"Cannot edit row. Existing row with ID {row.Id?.ToString() ?? "N/A"} could not be found.");

        rowToEdit.Account = new Account(row.Account, row.Subsidiary);
        rowToEdit.Money = new Money(row.Amount, row.CurrencyCode, row.CurrencyRate);
        rowToEdit.Authorizer = new Authorizer(row.Authorizer, false);
        rowToEdit.BusinessUnit = new BusinessUnit(row.BusinessUnit);
        rowToEdit.CostObject1 = new CostObject(1, row.CostObject1, row.CostObjectType1);
        rowToEdit.CostObject2 = new CostObject(2, row.CostObject2, row.CostObjectType2);
        rowToEdit.CostObject3 = new CostObject(3, row.CostObject3, row.CostObjectType3);
        rowToEdit.CostObject4 = new CostObject(4, row.CostObject4, row.CostObjectType4);
        rowToEdit.Remark = new Remark(row.Remark);
        rowToEdit.Subledger = new Subledger(row.Subledger, row.SubledgerType);
    }

    public void DeleteRow(int rowId)
    {
        if (BookingStatus.Status != WiniStatus.Saved)
            throw new DomainLogicException("Cannot delete row. Rows can only be deleted when Booking status is 'Saved'.");

        var rowToDelete = Rows.SingleOrDefault(_ => _.RowId?.Value == rowId);

        if (rowToDelete == null)
            throw new DomainLogicException($"Cannot delete row. Existing row with ID {rowId} could not be found.");

        Rows.Remove(rowToDelete);
    }

    private BookingRow MapBookingRowModelToValue(BookingRowModel row)
    => new BookingRow(
        row.Id.HasValue ? new IdValue<int>(row.Id.Value) : default,
        BookingId,
        new BusinessUnit(row.BusinessUnit),
        new Account(row.Account, row.Subsidiary),
        new Subledger(row.Subledger, row.SubledgerType),
        new CostObject(1, row.CostObject1, row.CostObjectType1),
        new CostObject(2, row.CostObject2, row.CostObjectType2),
        new CostObject(3, row.CostObject3, row.CostObjectType3),
        new CostObject(4, row.CostObject4, row.CostObjectType4),
        new Remark(row.Remark),
        new Authorizer(row.Authorizer, false),
        new Money(row.Amount, row.CurrencyCode, row.CurrencyRate)
    );
}