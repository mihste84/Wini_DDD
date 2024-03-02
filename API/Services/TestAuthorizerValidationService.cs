namespace API.Services;

public class TestAuthorizerValidationService : IAuthorizerValidationService
{
    public Task<(bool IsValid, IEnumerable<ValidationError> Errors)> CanAuthorizeBookingRows(IEnumerable<BookingRow> rows)
    => Task.FromResult<(bool IsValid, IEnumerable<ValidationError> Errors)>(
        (IsValid: true, Errors: Array.Empty<ValidationError>())
    );
}