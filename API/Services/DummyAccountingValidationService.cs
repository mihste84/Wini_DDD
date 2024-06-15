namespace API.Services;

public class DummyAccountingValidationService : IAccountingValidationService
{
    public Task<(bool IsValid, IEnumerable<ValidationError> Errors)> ValidateAsync(IEnumerable<AccountingValidationInputModel> input)
    => Task.FromResult<(bool IsValid, IEnumerable<ValidationError> Errors)>(
        (IsValid: true, Errors: Array.Empty<ValidationError>())
    );
}