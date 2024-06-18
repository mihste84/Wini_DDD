using Domain.Common.Enums;
using Domain.Wini.Aggregates;
using Domain.Wini.Enums;
using Domain.Wini.Events;
using Domain.Wini.Values;
using Microsoft.EntityFrameworkCore;

namespace DatabaseEf.Repositories;

public class BookingRepository(WiniDbContext ctx) : IBookingRepository
{
    public Task DeleteBookingIdAsync(int? id)
    {
        return ctx.Bookings.Where(_ => _.Id == id).ExecuteDeleteAsync();
    }

    public Task<Attachment?> GetAttachmentAsync(int bookingId, string name)
    {
        throw new NotImplementedException();
    }

    public async Task<(Booking Booking, byte[] RowVersion, int[]? DeletedRows)?> GetBookingIdAsync(int? id, bool includeRows = true)
    {
        var baseQuery = ctx.Bookings
                .Include(b => b.Attachments)
                .Include(b => b.BookingStatusLogs.OrderByDescending(x => x.Created))
                .Include(b => b.Comments)
                .Include(b => b.RecipientMessages)
                .AsQueryable();

        if (includeRows)
        {
            baseQuery = baseQuery.Include(b => b.BookingRows);
        }

        var booking = await baseQuery.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id)
            ?? throw new DatabaseOperationException($"Failed to find booking with ID {id}.");


        return (
            ModelToDomain.MapToDomainModel(booking),
            booking.RowVersion.GetValue(nameof(booking.RowVersion)),
            booking.BookingRows.Where(_ => _.IsDeleted).Select(_ => _.RowNumber).ToArray()
        );
    }

    public async Task<SqlResult?> InsertBookingAsync(Booking booking, string user)
    {
        var bookingModel = DomainToModel.MapToModel(booking, user);
        ctx.Bookings.Add(bookingModel);

        var numberOfRowsAdded = await ctx.SaveChangesAsync();

        if (numberOfRowsAdded == 0)
        {
            throw new DatabaseOperationException("Failed to insert booking rows.");
        }

        await ctx.SaveChangesAsync();

        return new SqlResult
        {
            Id = bookingModel.Id.GetValueOrDefault(),
            RowVersion = bookingModel.RowVersion
        };
    }

    public async Task<bool> IsSameRowVersionAsync(int id, byte[] rowVersion)
    {
        var rv = (await ctx.Bookings.AsNoTracking().FirstOrDefaultAsync(_ => _.Id == id))?.RowVersion ?? default;
        return rv?.SequenceEqual(rowVersion) ?? false;
    }

    public Task<BookingSearchResult[]> SearchBookingsAsync(DynamicSearchQuery query)
    {
        throw new NotImplementedException();
    }

    public async Task<SqlResult?> UpdateBookingAsync(Booking booking, string user)
    {
        var bookingModel = DomainToModel.MapToModel(booking, user);
        await TryInsertStatusLogsAsync(booking, user);
        await TryUpdateRowsAsync(booking, bookingModel);
        await TryUpdateCommentsAsync(booking, bookingModel);
        await TryUpdateRecipinetMessageAsync(booking, bookingModel);
        await TryUpdateAttachmentsAsync(booking, bookingModel);

        ctx.Bookings.Update(bookingModel);
        await ctx.SaveChangesAsync();

        return new SqlResult
        {
            Id = bookingModel.Id.GetValueOrDefault(),
            RowVersion = bookingModel.RowVersion
        };
    }

    public async Task<SqlResult?> UpdateBookingStatusAsync(Booking booking, string user)
    {
        var bookingModel = DomainToModel.MapToModel(booking, user);

        await TryInsertStatusLogsAsync(booking, user);

        var rowActions = booking.DomainEvents.OfType<WiniBookingRowActionEvent>();
        if (rowActions.Any())
        {
            var authorizedRows = rowActions.Where(_ => _!.Action == BookingRowAction.Authorized).Select(_ => _.Row.RowNumber).ToArray();
            var removeAuthorization = rowActions.Any(_ => _!.Action == BookingRowAction.RemoveAuthorization);

            foreach (var row in bookingModel.BookingRows)
            {
                var rowToChange = await ctx.BookingRows
                    .FirstOrDefaultAsync(_ => _.RowNumber == row.RowNumber && _.BookingId == bookingModel.Id)
                    ?? throw new DatabaseOperationException($"Failed to find row with number {row.RowNumber}.");

                if (removeAuthorization)
                {
                    rowToChange.IsAuthorized = false;
                }

                if (authorizedRows.Length > 0 && authorizedRows.Contains(row.RowNumber))
                {
                    rowToChange.IsAuthorized = true;
                }
            }
        }

        ctx.Bookings.Update(bookingModel);
        await ctx.SaveChangesAsync();

        return new SqlResult
        {
            Id = bookingModel.Id.GetValueOrDefault(),
            RowVersion = bookingModel.RowVersion
        };
    }

    private async Task TryInsertStatusLogsAsync(Booking booking, string user)
    {
        var logs = booking.DomainEvents.OfType<WiniStatusEvent>()?
            .Select(_ => DomainToModel.MapToModel(_.Status, booking.BookingId!.Value, user));

        if (logs != default)
        {
            await ctx.BookingStatusLogs.AddRangeAsync(logs);
        }
    }

    private async Task TryUpdateCommentsAsync(Booking booking, DatabaseCommon.Models.Booking bookingModel)
    {
        if (booking.DomainEvents?.Exists(_ => _ is CommentActionEvent) == false)
        {
            return;
        }

        await ctx.Comments.Where(_ => _.BookingId == bookingModel.Id).ExecuteDeleteAsync();

        if (booking.Comments.Count > 0)
        {
            await ctx.Comments.AddRangeAsync(bookingModel.Comments);
        }
    }

    private async Task TryUpdateAttachmentsAsync(Booking booking, DatabaseCommon.Models.Booking bookingModel)
    {
        if (booking.DomainEvents?.Exists(_ => _ is AttachmentActionEvent) == false)
        {
            return;
        }

        var attachmentEvents = booking.DomainEvents!.OfType<AttachmentActionEvent>();
        var attachemntsToDelete = attachmentEvents
            .Where(_ => _!.Action == CrudAction.Deleted)
            .Select(_ => _.Attachment.Content.Name);

        if (attachemntsToDelete.Any())
        {
            await ctx.Attachments.Where(_ => attachemntsToDelete.Contains(_.Name)).ExecuteDeleteAsync();
        }

        var attachemntsToInsert = attachmentEvents
            .Where(_ => _!.Action == CrudAction.Added)
            .Select(_ => new DatabaseCommon.Models.Attachment
            {
                CreatedBy = bookingModel.CreatedBy,
                BookingId = _.Attachment.BookingId!.Value,
                ContentType = _.Attachment.Content.ContentType,
                Name = _.Attachment.Content.Name,
                Path = _.Attachment.Content.Path,
                Size = (int)_.Attachment.Content.Size
            });

        if (attachemntsToInsert.Any())
        {
            await ctx.Attachments.AddRangeAsync(attachemntsToInsert);
        }
    }

    private async Task TryUpdateRecipinetMessageAsync(Booking booking, DatabaseCommon.Models.Booking bookingModel)
    {
        if (booking.DomainEvents?.Exists(_ => _ is RecipinetMessageActionEvent) == false)
        {
            return;
        }

        // Few items per booking that are rarely updated. Just delete and insert all when changes are made.
        await ctx.RecipientMessages.Where(_ => _.BookingId == bookingModel.Id).ExecuteDeleteAsync();

        if (booking.Messages.Count > 0)
        {
            await ctx.RecipientMessages.AddRangeAsync(bookingModel.RecipientMessages);
        }
    }

    private async Task TryUpdateRowsAsync(Booking booking, DatabaseCommon.Models.Booking bookingModel)
    {
        var rowsToDelete = booking.DomainEvents.OfType<WiniBookingRowDeleteEvent>()?
            .Select(_ => _.RowNumber);

        if (rowsToDelete?.Any() == true)
        {
            await ctx.BookingRows
                .Where(_ => _.BookingId == bookingModel.Id && rowsToDelete.Contains(_.RowNumber))
                .ExecuteDeleteAsync();
        }

        var editedRows = booking.DomainEvents.OfType<WiniBookingRowActionEvent>()?
            .Select(_ => _.Row.RowNumber);

        foreach (var row in bookingModel.BookingRows)
        {
            var rowToChange = await ctx.BookingRows
                .FirstOrDefaultAsync(_ => _.RowNumber == row.RowNumber && _.BookingId == bookingModel.Id);

            if (rowToChange == default)
            {
                ctx.BookingRows.Add(row);
                continue;
            }
            else if (editedRows?.Any(_ => _ == rowToChange.RowNumber) == true)
            {
                rowToChange.Account = row.Account;
                rowToChange.Amount = row.Amount;
                rowToChange.Authorizer = row.Authorizer;
                rowToChange.CurrencyCode = row.CurrencyCode;
                rowToChange.BusinessUnit = row.BusinessUnit;
                rowToChange.CostObject1 = row.CostObject1;
                rowToChange.CostObject2 = row.CostObject2;
                rowToChange.CostObject3 = row.CostObject3;
                rowToChange.CostObject4 = row.CostObject4;
                rowToChange.CostObjectType1 = row.CostObjectType1;
                rowToChange.CostObjectType2 = row.CostObjectType2;
                rowToChange.CostObjectType3 = row.CostObjectType3;
                rowToChange.CostObjectType4 = row.CostObjectType4;
                rowToChange.ExchangeRate = row.ExchangeRate;
                rowToChange.Remark = row.Remark;
                rowToChange.RowNumber = row.RowNumber;
                rowToChange.Subledger = row.Subledger;
                rowToChange.SubledgerType = row.SubledgerType;
                rowToChange.Subsidiary = row.Subsidiary;
            }
        }
    }
}
