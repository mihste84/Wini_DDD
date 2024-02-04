namespace AppLogic.Extensions;

public static class RequestExtensions
{
    public static bool IsValid<M>(this AbstractValidator<M> validator, M value, out IEnumerable<ValidationError> errors)
        where M : new()
    {
        errors = Array.Empty<ValidationError>();

        var result = validator.Validate(value);
        if (result.IsValid) return true;

        errors = result.Errors.Select(_ => new ValidationError(_));
        return false;
    }
}