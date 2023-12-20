namespace Domain.Wini.Interfaces;

public interface IAuthorizerValidationService
{
    Task<(bool IsValid, IEnumerable<ValidationError> Errors)> CanAuthorizeBookingRows(IEnumerable<BookingRow> rows);
}