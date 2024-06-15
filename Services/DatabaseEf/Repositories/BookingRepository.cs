using Domain.Wini.Aggregates;
using Domain.Wini.Values;
using Microsoft.EntityFrameworkCore;

namespace Services.DatabaseEf.Repositories;

public class BookingRepository(WiniDbContext ctx) : IBookingRepository
{
    public Task DeleteBookingIdAsync(int? id)
    {
        var bookingToDelete = ctx.Bookings.Find(id)
            ?? throw new DatabaseOperationException($"Failed to delete booking with ID {id}.");

        ctx.Bookings.Remove(bookingToDelete);

        return ctx.SaveChangesAsync();
    }

    public Task<Attachment?> GetAttachmentAsync(int bookingId, string name)
    {
        throw new NotImplementedException();
    }

    public async Task<(Booking Booking, byte[] RowVersion, int[]? DeletedRows)?> GetBookingIdAsync(int? id, bool includeRows = true)
    {
        var baseBooking = await ctx.Bookings
                .Include(b => b.Attachments)
                .Include(b => b.BookingStatusLogs.OrderByDescending(x => x.Created))
                .Include(b => b.Comments)
                .Include(b => b.RecipientMessages)
            .FirstOrDefaultAsync(b => b.Id == id)
            ?? throw new DatabaseOperationException($"Failed to find booking with ID {id}.");
        throw new NotImplementedException();
    }

    public Task<SqlResult?> InsertBookingAsync(Booking booking, string user)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsSameRowVersionAsync(int id, byte[] rowVersion)
    {
        throw new NotImplementedException();
    }

    public Task<BookingSearchResult[]> SearchBookingsAsync(DynamicSearchQuery query)
    {
        throw new NotImplementedException();
    }

    public Task<SqlResult?> UpdateBookingAsync(Booking booking, string user)
    {
        throw new NotImplementedException();
    }

    public Task<SqlResult?> UpdateBookingStatusAsync(Booking booking, string user)
    {
        throw new NotImplementedException();
    }
}
