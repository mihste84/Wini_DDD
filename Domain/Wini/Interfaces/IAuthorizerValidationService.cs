namespace Domain.Wini.Interfaces;

public interface IAuthorizerValidationService
{
    Task<(bool IsValid, string ErrorMessage)> CanAuthorizeBookingRow(BookingRow row);
}