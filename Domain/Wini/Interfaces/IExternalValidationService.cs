namespace Domain.Wini.Interfaces;

public interface IExternalValidationService
{
    Task<(bool IsValid, string[] ErrorMessages)> ValidateBooking(Booking booking);
}