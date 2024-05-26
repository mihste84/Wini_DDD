namespace Domain.Wini.Interfaces;

public interface IBookingPeriodValidationService
{
    Task<(bool IsValid, IEnumerable<ValidationError> Errors)> ValidateAsync(Booking booking);
}