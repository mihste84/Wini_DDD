namespace DatabaseCommon.Mappings;
public static class ModelToDomain
{
    public static Booking MapToDomainModel(
        Models.Booking booking,
        IEnumerable<Models.BookingRow> rows,
        IEnumerable<Models.Comment> comments,
        IEnumerable<Models.RecipientMessage> messages,
        IEnumerable<Models.BookingStatusLog> logs,
        IEnumerable<Models.Attachment> attachments
    )
    => new(
        new IdValue<int>(booking.Id.GetValue<int>(nameof(booking.Id))),
        new BookingStatus(
            (WiniStatus)booking.Status,
            booking.Updated,
            booking.UpdatedBy.GetValue(nameof(booking.UpdatedBy)),
            logs.Select(MapToDomainModel).ToList()
        ),
        new Commissioner(booking.CreatedBy.GetValue(nameof(booking.CreatedBy))),
        new BookingDate(booking.BookingDate),
        new TextToE1(booking.TextToE1),
        booking.Reversed,
        new LedgerType((Ledgers)booking.LedgerType),
        rows.Select(MapToDomainModel).ToList(),
        comments.Select(MapToDomainModel).ToList(),
        messages.Select(MapToDomainModel).ToList(),
        attachments.Select(MapToDomainModel).ToList(),
        booking.Created
    );

    public static Booking MapToDomainModel(
        Models.Booking booking
    )
    => new(
        new IdValue<int>(booking.Id.GetValue<int>(nameof(booking.Id))),
        new BookingStatus(
            (WiniStatus)booking.Status,
            booking.Updated,
            booking.UpdatedBy.GetValue(nameof(booking.UpdatedBy)),
            booking.BookingStatusLogs.Select(MapToDomainModel).ToList()
        ),
        new Commissioner(booking.CreatedBy.GetValue(nameof(booking.CreatedBy))),
        new BookingDate(booking.BookingDate),
        new TextToE1(booking.TextToE1),
        booking.Reversed,
        new LedgerType((Ledgers)booking.LedgerType),
        booking.BookingRows.Where(_ => !_.IsDeleted).Select(MapToDomainModel).ToList(),
        booking.Comments.Select(MapToDomainModel).ToList(),
        booking.RecipientMessages.Select(MapToDomainModel).ToList(),
        booking.Attachments.Select(MapToDomainModel).ToList(),
        booking.Created
    );

    public static BookingStatus MapToDomainModel(Models.BookingStatusLog log)
    => new(
        (WiniStatus)log.Status,
        log.Created,
        log.CreatedBy.GetValue(nameof(log.CreatedBy))
    );

    public static Attachment MapToDomainModel(Models.Attachment attachment)
    => new(
        BookingType.Wini,
        new IdValue<int>(attachment.BookingId),
        new FileContent(
            attachment.Size,
            attachment.ContentType.GetValue(nameof(attachment.ContentType)),
            attachment.Name.GetValue(nameof(attachment.Name)),
            attachment.Path.GetValue(nameof(attachment.Path))
        )
    );

    public static RecipientMessage MapToDomainModel(Models.RecipientMessage msg)
    => new(
        msg.Value.GetValue(nameof(msg.Value)),
        msg.Recipient.GetValue(nameof(msg.Recipient)),
        new IdValue<int>(msg.BookingId)
    );

    public static Comment MapToDomainModel(Models.Comment comment)
    => new(
        comment.Value,
        new IdValue<int>(comment.BookingId),
        comment.Created
    );

    public static BookingRow MapToDomainModel(Models.BookingRow row)
    => new(
        row.RowNumber,
        new BusinessUnit(row.BusinessUnit),
        new Account(row.Account, row.Subsidiary),
        new Subledger(row.Subledger, row.SubledgerType),
        new CostObject(1, row.CostObject1, row.CostObjectType1),
        new CostObject(2, row.CostObject2, row.CostObjectType2),
        new CostObject(3, row.CostObject3, row.CostObjectType3),
        new CostObject(4, row.CostObject4, row.CostObjectType4),
        new Remark(row.Remark),
        new Authorizer(row.Authorizer, row.IsAuthorized),
        new Money(row.Amount, row.CurrencyCode, row.ExchangeRate)
    );
}