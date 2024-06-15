namespace API.Services;

public class DummyBookingPeriodValidationService : IBookingPeriodValidationService
{
    public Task<(bool IsValid, IEnumerable<ValidationError> Errors)> ValidateAsync(Booking booking)
    => Task.FromResult<(bool IsValid, IEnumerable<ValidationError> Errors)>(
        (IsValid: true, Errors: Array.Empty<ValidationError>())
    );
}