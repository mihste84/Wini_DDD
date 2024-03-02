namespace AppLogic.Interfaces;

public interface IBookingRepository
{
    Task<(Booking Booking, byte[] RowVersion)?> GetBookingIdAsync(int? id, bool includeRows = true);
    Task<SqlResult?> InsertBookingAsync(Booking booking, string user);
    Task<SqlResult?> UpdateBookingAsync(Booking booking, string user);
    Task<SqlResult?> UpdateBookingStatusAsync(Booking booking, string user);
    Task<bool> IsSameRowVersionAsync(int id, byte[] rowVersion);
    Task DeleteBookingIdAsync(int? id);
}