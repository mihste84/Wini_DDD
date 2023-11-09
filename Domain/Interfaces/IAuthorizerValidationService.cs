namespace Domain.Interfaces;

public interface IAuthorizerValidationService
{
    Task<(bool IsValid, string ErrorMessage)> CanAuthorizeBookingRow(BookingRow row);
}