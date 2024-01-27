namespace Domain.Wini.Interfaces;

public interface IBookingValidationService
{
    Task<BookingValidationResultModel> ValidateAsync(Booking booking, IEnumerable<Company> companies);
}