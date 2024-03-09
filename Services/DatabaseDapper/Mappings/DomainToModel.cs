using System.Collections.Immutable;

namespace Services.DatabaseDapper.Mappings;

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
        new IdValue<int>(booking.Id.GetValue(nameof(booking.Id))),
        new BookingStatus(
            (WiniStatus)booking.Status.GetValue(nameof(booking.Status)),
            booking.Updated.GetValue(nameof(booking.Updated)),
            booking.UpdatedBy.GetValue(nameof(booking.UpdatedBy)),
            logs.Select(MapToDomainModel).ToList()
        ),
        new Commissioner(booking.CreatedBy.GetValue(nameof(booking.CreatedBy))),
        new BookingDate(booking.BookingDate.GetValue(nameof(booking.BookingDate))),
        new TextToE1(booking.TextToE1),
        booking.Reversed.GetValue(nameof(booking.Reversed)),
        new LedgerType((Ledgers)booking.LedgerType.GetValue(nameof(booking.LedgerType))),
        rows.Select(MapToDomainModel).ToList(),
        comments.Select(MapToDomainModel).ToList(),
        messages.Select(MapToDomainModel).ToList(),
        attachments.Select(MapToDomainModel).ToList(),
        booking.Created.GetValue(nameof(booking.Created))
    );

    public static BookingStatus MapToDomainModel(Models.BookingStatusLog log)
    => new(
        (WiniStatus)log.Status.GetValue(nameof(log.Status)),
        log.Created.GetValue(nameof(log.Created)),
        log.CreatedBy.GetValue(nameof(log.CreatedBy))
    );

    public static Attachment MapToDomainModel(Models.Attachment attachment)
    => new(
        BookingType.Wini,
        new IdValue<int>(attachment.BookingId.GetValue(nameof(attachment.BookingId))),
        new FileContent(
            attachment.Size.GetValue(nameof(attachment.Size)),
            attachment.ContentType.GetValue(nameof(attachment.ContentType)),
            attachment.Name.GetValue(nameof(attachment.Name)),
            attachment.Path.GetValue(nameof(attachment.Path))
        )
    );

    public static RecipientMessage MapToDomainModel(Models.RecipientMessage msg)
    => new(
        msg.Value.GetValue(nameof(msg.Value)),
        msg.Recipient.GetValue(nameof(msg.Recipient)),
        new IdValue<int>(msg.BookingId.GetValue(nameof(msg.BookingId)))
    );

    public static Comment MapToDomainModel(Models.Comment comment)
    => new(
        comment.Value,
        new IdValue<int>(comment.BookingId!.Value),
        comment.Created.GetValue(nameof(comment.Created))
    );

    public static BookingRow MapToDomainModel(Models.BookingRow row)
    => new(
        row.RowNumber.GetValue(nameof(row.RowNumber)),
        new BusinessUnit(row.BusinessUnit),
        new Account(row.Account, row.Subsidiary),
        new Subledger(row.Subledger, row.SubledgerType),
        new CostObject(1, row.CostObject1, row.CostObjectType1),
        new CostObject(2, row.CostObject2, row.CostObjectType2),
        new CostObject(3, row.CostObject3, row.CostObjectType3),
        new CostObject(4, row.CostObject4, row.CostObjectType4),
        new Remark(row.Remark),
        new Authorizer(row.Authorizer, row.IsAuthorized.GetValueOrDefault()),
        new Money(row.Amount, row.CurrencyCode, row.ExchangeRate)
    );
}