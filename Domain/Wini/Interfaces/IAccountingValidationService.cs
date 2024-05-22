namespace Domain.Wini.Interfaces;

public interface IAccountingValidationService
{
    Task<(bool IsValid, IEnumerable<ValidationError> Errors)> ValidateAsync(IEnumerable<AccountingValidationInputModel> input);
}