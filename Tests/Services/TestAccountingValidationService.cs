namespace Tests.MockServices;

public class TestAccountingValidationService : IAccountingValidationService
{
    public Task<(bool IsValid, IEnumerable<ValidationError> Errors)> ValidateAsync(IEnumerable<AccountingValidationInputModel> input)
    => Task.FromResult<(bool IsValid, IEnumerable<ValidationError> Errors)>(
        (IsValid: true, Errors: Array.Empty<ValidationError>())
    );
}