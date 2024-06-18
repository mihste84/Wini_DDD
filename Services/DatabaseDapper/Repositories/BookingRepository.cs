namespace DatabaseDapper.Repositories;

public class BookingRepository(ConnectionFactory factory) : IBookingRepository
{
    public async Task<BookingSearchResult[]> SearchBookingsAsync(DynamicSearchQuery query)
    {
        using var conn = factory.CreateConnection();
        conn.Open();

        var sql = conn.QueryBuilder(
            BookingQueries.GetSearchBookingsQuery(
                query.StartRow,
                query.EndRow,
                query.OrderBy,
                query.OrderByDirection
            )
        );

        void trySetDateWhere(string propName, string columnName)
        {
            if (query.TryExtractSearchItemByName(propName, out var searchItem))
            {
                var newSearchItem = new SearchItem(columnName, searchItem!.Value, searchItem!.Operator);
                var whereStatement = DynamicSearchQuery.GetWhereFromSearchItem(newSearchItem);
                sql.Where(whereStatement);
            }
        }

        void trySetWhereForRow(string name)
        {
            if (query.TryExtractSearchItemByName(name, out var searchItem))
            {
                var rowSql = DynamicSearchQuery.GetWhereFromSearchItem(searchItem!);
                sql.Where($"EXISTS (SELECT 1 FROM dbo.BookingRows WHERE BookingId = x.Id AND IsDeleted = 0 AND {rowSql})");
            }
        }

        foreach (var searchItem in query.SearchItems.Where(_ => _.HandleAutomatically))
        {
            sql.Where(DynamicSearchQuery.GetWhereFromSearchItem(searchItem));
        }

        foreach (var propName in new[] { "FromCreatedDate", "ToCreatedDate" })
        {
            trySetDateWhere(propName, "Created");
        }

        foreach (var propName in new[] { "FromBookingDate", "ToBookingDate" })
        {
            trySetDateWhere(propName, "BookingDate");
        }

        foreach (var columnName in new[] { "BusinessUnit", "Account", "Authorizer", "Amount" })
        {
            trySetWhereForRow(columnName);
        }

        var res = await sql.QueryAsync<DatabaseCommon.Models.DbBookingSearchResult>();
        if (res?.Any() == false)
        {
            return [];
        }

        return res!.Select(_ => new BookingSearchResult(
            _.Id.GetValue(nameof(_.Id)),
            DateOnly.FromDateTime(_.BookingDate.GetValue(nameof(_.BookingDate))),
            (WiniStatus)_.Status.GetValue(nameof(_.Status)),
            _.CreatedBy.GetValue(nameof(_.CreatedBy)),
            _.AttachmentsCount.GetValueOrDefault(),
            _.Comments,
            _.Created.GetValue(nameof(_.Created))
        )).ToArray();
    }

    public async Task<Attachment?> GetAttachmentAsync(int bookingId, string name)
    {
        using var conn = factory.CreateConnection();
        conn.Open();

        var attachment = await conn.QueryFirstOrDefaultAsync<DatabaseCommon.Models.Attachment>(
            AttachmentQueries.SelectByBookingIdAndName,
            new { BookingId = bookingId, Name = name });
        if (attachment == default)
        {
            return default;
        }

        return ModelToDomain.MapToDomainModel(attachment);
    }
    public async Task<(Booking Booking, byte[] RowVersion, int[]? DeletedRows)?> GetBookingIdAsync(int? id, bool includeRows = true)
    {
        using var conn = factory.CreateConnection();
        conn.Open();

        var query = includeRows ? BookingQueries.SelectBookingAndRowsById : BookingQueries.SelectBookingById;
        var mapper = await conn.QueryMultipleAsync(query, new { BookingId = id });

        var booking = await mapper.ReadFirstOrDefaultAsync<DatabaseCommon.Models.Booking>();
        if (booking == default)
        {
            return default;
        }

        var comments = await mapper.ReadAsync<DatabaseCommon.Models.Comment>();
        var messages = await mapper.ReadAsync<DatabaseCommon.Models.RecipientMessage>();
        var logs = await mapper.ReadAsync<DatabaseCommon.Models.BookingStatusLog>();
        var attachments = await mapper.ReadAsync<DatabaseCommon.Models.Attachment>();
        var rows = includeRows ? await mapper.ReadAsync<DatabaseCommon.Models.BookingRow>() : [];
        var deletedRows = includeRows ? await mapper.ReadAsync<int>() : [];

        return (
            ModelToDomain.MapToDomainModel(booking, rows, comments, messages, logs, attachments),
            booking.RowVersion.GetValue(nameof(booking.RowVersion)),
            deletedRows?.ToArray()
        );
    }

    public async Task DeleteBookingIdAsync(int? id)
    {
        using var conn = factory.CreateConnection();
        conn.Open();

        if (await conn.ExecuteAsync(BookingQueries.DeleteBookingById, new { BookingId = id }) != 0)
        {
            return;
        }

        throw new DatabaseOperationException($"Failed to delete booking with ID {id}.");
    }

    public async Task<bool> IsSameRowVersionAsync(int id, byte[] rowVersion)
    {
        using var conn = factory.CreateConnection();
        conn.Open();

        var res = await conn.QueryFirstAsync<byte[]>(BookingQueries.SelectRowVersionById, new { Id = id });

        return rowVersion.SequenceEqual(res);
    }

    public async Task<SqlResult?> UpdateBookingStatusAsync(Booking booking, string user)
    {
        var bookingModel = DomainToModel.MapToModel(booking, user);

        using var conn = factory.CreateConnection();
        conn.Open();

        var res = await conn.QuerySingleAsync<SqlResult>(BookingQueries.UpdateStatus, bookingModel);
        await TryInsertStatusLogsAsync(booking, user, conn);

        var rowActions = booking.DomainEvents.OfType<WiniBookingRowActionEvent>();
        if (!rowActions.Any())
        {
            return res;
        }

        if (rowActions.Any(_ => _!.Action == BookingRowAction.RemoveAuthorization))
        {
            await conn.ExecuteAsync(BookingRowQueries.UpdateRemoveAllAuthorizations, new { BookingId = res.Id }); // No need to check number of updated rows. Can be 0.
        }

        await TryAuthorizeRowsAsync(rowActions, res.Id, conn);

        return res;
    }

    public async Task<SqlResult?> UpdateBookingAsync(Booking booking, string user)
    {
        var bookingModel = DomainToModel.MapToModel(booking, user);

        using var conn = factory.CreateConnection();
        conn.Open();

        var res = await conn.QuerySingleAsync<SqlResult>(BookingQueries.Update, bookingModel);

        await TryInsertStatusLogsAsync(booking, user, conn);
        await TryDeleteRowsAsync(booking, conn);
        await TryInsertRowsAsync(booking, conn);
        await TryUpdateCommentsAsync(booking, user, conn);
        await TryUpdateRecipinetMessageAsync(booking, user, conn);
        await TryUpdateAttachmentsAsync(booking, user, conn);

        return res;
    }

    public async Task<SqlResult?> InsertBookingAsync(Booking booking, string user)
    {
        var bookingModel = DomainToModel.MapToModel(booking, user);

        using var conn = factory.CreateConnection();
        conn.Open();

        var res = await conn.QuerySingleAsync<SqlResult>(BookingQueries.Insert, bookingModel);
        await TryInsertStatusLogsAsync(booking, user, conn);

        if (booking.Rows.Count == 0)
        {
            return res;
        }

        var rowsToInsert = booking.Rows.Select(_ => DomainToModel.MapToModel(_, res.Id));
        var numberOfRowsAdded = await conn.ExecuteAsync(BookingRowQueries.Insert, rowsToInsert);
        if (numberOfRowsAdded == 0)
        {
            throw new DatabaseOperationException("Failed to insert booking rows.");
        }

        return res;
    }

    private static async Task TryUpdateAttachmentsAsync(Booking booking, string user, IDbConnection conn)
    {
        if (booking.DomainEvents?.Exists(_ => _ is AttachmentActionEvent) == false)
        {
            return;
        }

        var attachmentEvents = booking.DomainEvents!.OfType<AttachmentActionEvent>();

        await TryInsertAttachmentsAsync(attachmentEvents, user, conn);
        await TryDeleteAttachmentsAsync(attachmentEvents, booking.BookingId!.Value, conn);
    }

    private static async Task TryInsertAttachmentsAsync(
        IEnumerable<AttachmentActionEvent> events,
        string user,
        IDbConnection conn
    )
    {
        var attachemntsToInsert = events
            .Where(_ => _!.Action == CrudAction.Added)
            .Select(_ => new
            {
                CreatedBy = user,
                BookingId = _.Attachment.BookingId!.Value,
                _.Attachment.Content.ContentType,
                _.Attachment.Content.Name,
                _.Attachment.Content.Path,
                _.Attachment.Content.Size
            });

        if (!attachemntsToInsert.Any())
        {
            return;
        }
        var numberOfRowsAdded = await conn.ExecuteAsync(AttachmentQueries.Insert, attachemntsToInsert);
        if (numberOfRowsAdded == 0)
        {
            throw new DatabaseOperationException("Failed to insert attachments.");
        }
    }

    private static async Task TryDeleteAttachmentsAsync(
        IEnumerable<AttachmentActionEvent> events,
        int bookingId,
        IDbConnection conn
    )
    {
        var attachemntsToDelete = events
            .Where(_ => _!.Action == CrudAction.Deleted)
            .Select(_ => _.Attachment.Content.Name);

        if (!attachemntsToDelete.Any())
        {
            return;
        }

        var numberOfRowsDeleted = await conn.ExecuteAsync(
            AttachmentQueries.DeleteByBookingName,
            new { BookingId = bookingId, Names = attachemntsToDelete }
        );

        if (numberOfRowsDeleted == 0)
        {
            throw new DatabaseOperationException("Failed to delete attachments.");
        }
    }

    private static async Task TryAuthorizeRowsAsync(
        IEnumerable<WiniBookingRowActionEvent?> rowActions,
        int bookingId,
        IDbConnection conn
    )
    {
        var authorizedRows = rowActions.Where(_ => _!.Action == BookingRowAction.Authorized);
        var authorizedRowsCount = authorizedRows.Count();
        if (authorizedRowsCount == 0)
        {
            return;
        }

        var model = new { Rows = authorizedRows.Select(_ => _!.Row.RowNumber), BookingId = bookingId };
        var numberOfRowsAuthorized = await conn.ExecuteAsync(BookingRowQueries.UpdateAuthorizeByRow, model);
        if (numberOfRowsAuthorized != authorizedRowsCount)
        {
            throw new DatabaseOperationException("Failed to authorize all booking rows.");
        }
    }

    private static async Task TryUpdateRecipinetMessageAsync(Booking booking, string user, IDbConnection conn)
    {
        if (booking.DomainEvents?.Exists(_ => _ is RecipinetMessageActionEvent) == false)
        {
            return;
        }

        // Few items per booking that are rarely updated. Just delete and insert all when changes are made.
        await conn.ExecuteAsync(RecipientMessageQueries.DeleteAllByBookingId, new { BookingId = booking.BookingId!.Value });

        if (booking.Messages.Count > 0)
        {
            var messagesToInsert = booking.Messages.Select(_ => DomainToModel.MapToModel(_, user));
            await conn.ExecuteAsync(RecipientMessageQueries.Insert, messagesToInsert);
        }
    }

    private static async Task TryUpdateCommentsAsync(Booking booking, string user, IDbConnection conn)
    {
        if (booking.DomainEvents?.Exists(_ => _ is CommentActionEvent) == false)
        {
            return;
        }

        // Few items per booking that are rarely updated. Just delete and insert all when changes are made.
        await conn.ExecuteAsync(CommentQueries.DeleteAllByBookingId, new { BookingId = booking.BookingId!.Value });

        if (booking.Comments.Count > 0)
        {
            var commentsToInsert = booking.Comments.Select(_ => DomainToModel.MapToModel(_, user));
            await conn.ExecuteAsync(CommentQueries.Insert, commentsToInsert);
        }
    }

    private static async Task TryInsertRowsAsync(Booking booking, IDbConnection conn)
    {
        var rowActions = booking.DomainEvents.OfType<WiniBookingRowActionEvent>();
        if (!rowActions.Any())
        {
            return;
        }

        var rowsToUpsert = rowActions.Select(_ => DomainToModel.MapToModel(_!.Row, booking.BookingId!.Value));
        var numberOfRowsUpserted = await conn.ExecuteAsync(BookingRowQueries.Upsert, rowsToUpsert);
        if (numberOfRowsUpserted == 0)
        {
            throw new DatabaseOperationException("Failed to upsert booking rows.");
        }
    }

    private static async Task TryDeleteRowsAsync(Booking booking, IDbConnection conn)
    {
        var rowsToDelete = booking.DomainEvents.OfType<WiniBookingRowDeleteEvent>();
        if (!rowsToDelete.Any())
        {
            return;
        }

        var deletedRows = await conn.ExecuteAsync(
            BookingRowQueries.UpdateIsDeleted,
            new { Rows = rowsToDelete.Select(_ => _!.RowNumber), BookingId = booking.BookingId!.Value, IsDeleted = true }
        );
        if (deletedRows == 0)
        {
            throw new DatabaseOperationException("Failed to delete booking rows.");
        }
    }

    private static async Task TryInsertStatusLogsAsync(Booking booking, string user, IDbConnection conn)
    {
        var statusChanges = booking.DomainEvents.OfType<WiniStatusEvent>();
        if (statusChanges.Any())
        {
            var statusesToAdd = statusChanges.Select(_ => DomainToModel.MapToModel(_!.Status, booking.BookingId!.Value, user));
            var numberOfRowsAdded = await conn.ExecuteAsync(BookingStatusLogQueries.Insert, statusesToAdd);
            if (numberOfRowsAdded == 0)
            {
                throw new DatabaseOperationException("Failed to insert status logs.");
            }
        }
    }
}
