namespace AppLogic.Behaviours;

[Obsolete("Not used because exceptions not used for flow control.")]
public class RequestValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public RequestValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);

        var failures = _validators
            .Select(v => v.Validate(context))
            .Where(_ => !_.IsValid)
            .SelectMany(_ => _.Errors)
            .Select(_ => new ValidationError(_));

        return failures.Any() ? throw new AppLogicValidationException(failures) : next();
    }
}