namespace Services.DatabaseDapper.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ConnectionFactory _factory;

    public BookingRepository(ConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<(Booking Booking, byte[] RowVersion)?> GetBookingIdAsync(int? id, bool includeRows = true)
    {
        using var conn = _factory.CreateConnection();
        conn.Open();

        var query = includeRows ? BookingQueries.SelectBookingAndRowsById : BookingQueries.SelectBookingById;
        var mapper = await conn.QueryMultipleAsync(query, new { BookingId = id });

        var booking = await mapper.ReadFirstOrDefaultAsync<Models.Booking>();
        if (booking == default) return default;

        var comments = await mapper.ReadAsync<Models.Comment>();
        var messages = await mapper.ReadAsync<Models.RecipientMessage>();
        var logs = await mapper.ReadAsync<Models.BookingStatusLog>();
        var attachments = await mapper.ReadAsync<Models.Attachment>();
        var rows = includeRows ? await mapper.ReadAsync<Models.BookingRow>() : Array.Empty<Models.BookingRow>();

        return (
            MapToDomainModel(booking, rows, comments, messages, logs, attachments),
            booking.RowVersion.GetValue(nameof(booking.RowVersion))
        );
    }

    public async Task DeleteBookingIdAsync(int? id)
    {
        using var conn = _factory.CreateConnection();
        conn.Open();

        if (await conn.ExecuteAsync(BookingQueries.DeleteBookingById, new { BookingId = id }) == 0)
            throw new DatabaseOperationException($"Failed to delete booking with ID {id}.");
    }

    public Task<(Booking Booking, byte[] RowVersion)?> GetEntireBookingByIdAsync(int? id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IsSameRowVersionAsync(int id, byte[] rowVersion)
    {
        using var conn = _factory.CreateConnection();
        conn.Open();

        var res = await conn.QueryFirstAsync<byte[]>(BookingQueries.SelectRowVersionById, new { Id = id });

        return rowVersion.SequenceEqual(res);
    }

    public async Task<SqlResult?> UpdateBookingStatusAsync(Booking booking, string user)
    {
        var bookingModel = MapToModel(booking, user);

        using var conn = _factory.CreateConnection();
        conn.Open();

        var res = await conn.QuerySingleAsync<SqlResult>(BookingQueries.UpdateStatus, bookingModel);
        await TryInsertStatusLogsAsync(booking, user, conn);

        var rowActions = booking.DomainEvents.Select(_ => _ as WiniBookingRowActionEvent).Where(_ => _ != default);
        if (!rowActions.Any()) return res;

        if (rowActions.Any(_ => _!.Action == BookingRowAction.RemoveAuthorization))
            await conn.ExecuteAsync(BookingRowQueries.UpdateRemoveAllAuthorizations, new { BookingId = res.Id }); // No need to check number of updated rows. Can be 0.

        await TryAuthorizeRowsAsync(rowActions, res.Id, conn);

        return res;
    }

    public async Task<SqlResult?> UpdateBookingAsync(Booking booking, string user)
    {
        var bookingModel = MapToModel(booking, user);

        using var conn = _factory.CreateConnection();
        conn.Open();

        var res = await conn.QuerySingleAsync<SqlResult>(BookingQueries.Update, bookingModel);

        await TryInsertStatusLogsAsync(booking, user, conn);
        await TryDeleteRowsAsync(booking, conn);
        await TryInsertRowsAsync(booking, conn);
        await TryUpdateCommentsAsync(booking, user, conn);
        await TryUpdateRecipinetMessageAsync(booking, user, conn);

        return res;
    }

    public async Task<SqlResult?> InsertBookingAsync(Booking booking, string user)
    {
        var bookingModel = MapToModel(booking, user);

        using var conn = _factory.CreateConnection();
        conn.Open();

        var res = await conn.QuerySingleAsync<SqlResult>(BookingQueries.Insert, bookingModel);
        await TryInsertStatusLogsAsync(booking, user, conn);

        if (booking.Rows.Count > 0)
        {
            var rowsToInsert = booking.Rows.Select(_ => MapToModel(_, res.Id));
            var numberOfRowsAdded = await conn.ExecuteAsync(BookingRowQueries.Insert, rowsToInsert);
            if (numberOfRowsAdded == 0) throw new DatabaseOperationException("Failed to insert booking rows.");
        }

        return res;
    }

    private static async Task TryAuthorizeRowsAsync(IEnumerable<WiniBookingRowActionEvent?> rowActions, int bookingId, IDbConnection conn)
    {
        var authorizedRows = rowActions.Where(_ => _!.Action == BookingRowAction.Authorized);
        var authorizedRowsCount = authorizedRows.Count();
        if (authorizedRowsCount > 0)
        {
            var model = new { Rows = authorizedRows.Select(_ => _!.Row.RowNumber), BookingId = bookingId };
            var numberOfRowsAuthorized = await conn.ExecuteAsync(BookingRowQueries.UpdateAuthorizeByRow, model);
            if (numberOfRowsAuthorized != authorizedRowsCount)
                throw new DatabaseOperationException("Failed to authorize all booking rows.");
        }
    }

    private static async Task TryUpdateRecipinetMessageAsync(Booking booking, string user, IDbConnection conn)
    {
        if (booking.DomainEvents?.Any(_ => _ is RecipinetMessageActionEvent) == false)
            return;
        // Few items per booking that are rarely updated. Just delete and insert all when changes are made.
        await conn.ExecuteAsync(RecipientMessageQueries.DeleteAllByBookingId, new { BookingId = booking.BookingId!.Value });

        if (booking.Messages.Count > 0)
        {
            var messagesToInsert = booking.Messages.Select(_ => MapToModel(_, user));
            await conn.ExecuteAsync(RecipientMessageQueries.Insert, messagesToInsert);
        }
    }

    private static async Task TryUpdateCommentsAsync(Booking booking, string user, IDbConnection conn)
    {
        if (booking.DomainEvents?.Any(_ => _ is CommentActionEvent) == false)
            return;
        // Few items per booking that are rarely updated. Just delete and insert all when changes are made.
        await conn.ExecuteAsync(CommentQueries.DeleteAllByBookingId, new { BookingId = booking.BookingId!.Value });

        if (booking.Comments.Count > 0)
        {
            var commentsToInsert = booking.Comments.Select(_ => MapToModel(_, user));
            await conn.ExecuteAsync(CommentQueries.Insert, commentsToInsert);
        }
    }


    private static async Task TryInsertRowsAsync(Booking booking, IDbConnection conn)
    {
        var rowActions = booking.DomainEvents.Select(_ => _ as WiniBookingRowActionEvent).Where(_ => _ != default);
        if (rowActions.Any())
        {
            var rowsToUpsert = rowActions.Select(_ => MapToModel(_!.Row, booking.BookingId!.Value));
            var numberOfRowsUpserted = await conn.ExecuteAsync(BookingRowQueries.Upsert, rowsToUpsert);
            if (numberOfRowsUpserted == 0) throw new DatabaseOperationException("Failed to upsert booking rows.");
        }
    }

    private static async Task TryDeleteRowsAsync(Booking booking, IDbConnection conn)
    {
        var rowsToDelete = booking.DomainEvents.Select(_ => _ as WiniBookingRowDeleteEvent).Where(_ => _ != default);
        if (rowsToDelete.Any())
        {
            var deletedRows = await conn.ExecuteAsync(
                BookingRowQueries.UpdateIsDeleted,
                new { Rows = rowsToDelete.Select(_ => _!.RowNumber), BookingId = booking.BookingId!.Value, IsDeleted = true }
            );
            if (deletedRows == 0) throw new DatabaseOperationException("Failed to delete booking rows.");
        }
    }

    private static async Task TryInsertStatusLogsAsync(Booking booking, string user, IDbConnection conn)
    {
        var statusChanges = booking.DomainEvents.Select(_ => _ as WiniStatusEvent).Where(_ => _ != default);
        if (statusChanges.Any())
        {
            var statusesToAdd = statusChanges.Select(_ => MapToModel(_!.Status, booking.BookingId!.Value, user));
            var numberOfRowsAdded = await conn.ExecuteAsync(BookingStatusLogQueries.Insert, statusesToAdd);
            if (numberOfRowsAdded == 0) throw new DatabaseOperationException("Failed to insert status logs.");
        }
    }

    private static Booking MapToDomainModel(
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

    private static BookingStatus MapToDomainModel(Models.BookingStatusLog log)
    => new(
        (WiniStatus)log.Status.GetValue(nameof(log.Status)),
        log.Created.GetValue(nameof(log.Created)),
        log.CreatedBy.GetValue(nameof(log.CreatedBy))
    );

    private static Attachment MapToDomainModel(Models.Attachment attachment)
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

    private static RecipientMessage MapToDomainModel(Models.RecipientMessage msg)
    => new(
        msg.Value.GetValue(nameof(msg.Value)),
        msg.Recipient.GetValue(nameof(msg.Recipient)),
        new IdValue<int>(msg.BookingId.GetValue(nameof(msg.BookingId)))
    );

    private static Comment MapToDomainModel(Models.Comment comment)
    => new(
        comment.Value,
        new IdValue<int>(comment.BookingId!.Value),
        comment.Created.GetValue(nameof(comment.Created))
    );

    private static BookingRow MapToDomainModel(Models.BookingRow row)
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

    private static Models.RecipientMessage MapToModel(RecipientMessage message, string user)
    => new()
    {
        BookingId = message.BookingId.Value,
        Value = message.Message,
        Recipient = message.Recipient.UserId,
        CreatedBy = user
        //Id = id,
    };

    private static Models.Comment MapToModel(Comment comment, string user)
    => new()
    {
        BookingId = comment.BookingId.Value,
        CreatedBy = user,
        Value = comment.Value,
        //Id = id,
        Created = comment.Created
    };

    private static Models.BookingStatusLog MapToModel(BookingStatus status, int bookingId, string user)
    => new()
    {
        BookingId = bookingId,
        Created = status.Updated,
        CreatedBy = user,
        Status = (short)status.Status
    };

    private static Models.Booking MapToModel(Booking booking, string user)
    => new()
    {
        BookingDate = booking.Header.BookingDate.Date,
        LedgerType = (short)booking.Header.LedgerType.Type,
        TextToE1 = booking.Header.TextToE1.Text,
        Reversed = booking.Header.IsReversed,
        Status = (short)booking.BookingStatus.Status,
        CreatedBy = booking.Commissioner.UserId,
        UpdatedBy = user,
        Id = booking.BookingId?.Value
    };

    private static Models.BookingRow MapToModel(BookingRow row, int bookingId)
    => new()
    {
        RowNumber = row.RowNumber,
        Account = row.Account.Value,
        Amount = row.Money.Amount,
        Authorizer = row.Authorizer.UserId,
        BookingId = bookingId,
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
}
