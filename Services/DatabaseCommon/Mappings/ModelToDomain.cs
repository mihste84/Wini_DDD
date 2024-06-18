namespace DatabaseCommon.Mappings;

public static class DomainToModel
{
    public static Models.RecipientMessage MapToModel(RecipientMessage message, string user)
     => new()
     {
         BookingId = message.BookingId.Value,
         Value = message.Message,
         Recipient = message.Recipient.UserId ?? throw new NullReferenceException(nameof(message.Recipient.UserId)),
         CreatedBy = user
         //Id = id,
     };

    public static Models.Comment MapToModel(Comment comment, string user)
    => new()
    {
        BookingId = comment.BookingId.Value,
        CreatedBy = user,
        Value = comment.Value ?? throw new NullReferenceException(nameof(comment.Value)),
        //Id = id,
        Created = comment.Created
    };

    public static Models.BookingStatusLog MapToModel(BookingStatus status, int bookingId, string user)
    => new()
    {
        BookingId = bookingId,
        Created = status.Updated,
        CreatedBy = user,
        Status = (short)status.Status
    };

    public static Models.Booking MapToModel(Booking booking, string user)
    => new()
    {
        BookingDate = booking.Header.BookingDate.Date.ToDateTime(default),
        LedgerType = (short)booking.Header.LedgerType.Type,
        TextToE1 = booking.Header.TextToE1.Text,
        Reversed = booking.Header.IsReversed,
        Status = (short)booking.BookingStatus.Status,
        CreatedBy = booking.Commissioner.UserId ?? throw new NullReferenceException(nameof(booking.Commissioner.UserId)),
        UpdatedBy = user,
        Updated = booking.BookingStatus.Updated,
        Id = booking.BookingId?.Value,
        BookingRows = booking.Rows?.Select(row => MapToModel(row)).ToList() ?? []
    };

    public static Models.BookingRow MapToModel(BookingRow row)
    => new()
    {
        RowNumber = row.RowNumber,
        Account = row.Account.Value,
        Amount = row.Money.Amount,
        Authorizer = row.Authorizer.UserId,
        BusinessUnit = row.BusinessUnit.ToString(),
        CostObject1 = row.CostObject1.Value,
        CostObject2 = row.CostObject2.Value,
        CostObject3 = row.CostObject3.Value,
        CostObject4 = row.CostObject4.Value,
        CostObjectType1 = row.CostObject1.Type,
        CostObjectType2 = row.CostObject2.Type,
        CostObjectType3 = row.CostObject3.Type,
        CostObjectType4 = row.CostObject4.Type,
        CurrencyCode = row.Money.Currency.CurrencyCode.Code,
        ExchangeRate = row.Money.Currency.ExchangeRate,
        IsAuthorized = row.Authorizer.HasAuthorized,
        Subledger = row.Subledger.Value,
        Remark = row.Remark.Text,
        SubledgerType = row.Subledger.Type,
        Subsidiary = row.Account.Subsidiary
    };

    public static Models.BookingRow MapToModel(BookingRow row, int bookingId)
    {
        var model = MapToModel(row);
        model.BookingId = bookingId;
        return model;
    }
}