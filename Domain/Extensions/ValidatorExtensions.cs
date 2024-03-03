namespace Domain.Extensions;

public static class RequestExtensions
{
    public static bool IsValid<M>(this AbstractValidator<M> validator, M value, out IEnumerable<ValidationError> errors)
        where M : class
    {
        errors = [];

        var result = validator.Validate(value);
        if (result.IsValid)
        {
            return true;
        }

        errors = result.Errors.Select(_ => new ValidationError(_));
        return false;
    }
}