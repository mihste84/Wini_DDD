
namespace AppLogic.Requests;

public class HelloCommand : IRequest<OneOf<Result<string>, ValidationErrorResult>>
{
    public string? Input { get; set; }

    public class HelloValidator : AbstractValidator<HelloCommand>
    {
        public HelloValidator()
        {
            RuleFor(_ => _.Input).NotEmpty().MinimumLength(5);
        }
    }

    public class HelloHandler : IRequestHandler<HelloCommand, OneOf<Result<string>, ValidationErrorResult>>
    {
        public Task<OneOf<Result<string>, ValidationErrorResult>> Handle(HelloCommand request, CancellationToken cancellationToken)
        {
            if (!new HelloValidator().IsValid(request, out var errors))
                return Task.FromResult<OneOf<Result<string>, ValidationErrorResult>>(new ValidationErrorResult(errors));

            return Task.FromResult<OneOf<Result<string>, ValidationErrorResult>>(new Result<string>("Hello " + request.Input!));
        }
    }
}