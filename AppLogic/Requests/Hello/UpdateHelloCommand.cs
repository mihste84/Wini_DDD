
namespace AppLogic.Requests;

public class UpdateHelloCommand : IRequest<OneOf<Result<SqlResult?>, ValidationErrorResult, Error<string>>>
{
    public int? Id { get; set; }
    public string? Input { get; set; }
    public byte[]? RowVersion { get; set; }

    public class UpdateHelloValidator : AbstractValidator<UpdateHelloCommand>
    {
        public UpdateHelloValidator()
        {
            RuleFor(_ => _.Id).NotEmpty();
            RuleFor(_ => _.Input).NotEmpty().MinimumLength(5);
            RuleFor(_ => _.RowVersion).NotEmpty();
        }
    }

    public class UpdateHelloHandler : IRequestHandler<UpdateHelloCommand, OneOf<Result<SqlResult?>, ValidationErrorResult, Error<string>>>
    {
        private readonly IHelloRepository _helloRepo;

        public UpdateHelloHandler(IHelloRepository helloRepo)
        {
            _helloRepo = helloRepo;
        }

        public async Task<OneOf<Result<SqlResult?>, ValidationErrorResult, Error<string>>> Handle(UpdateHelloCommand request, CancellationToken cancellationToken)
        {
            if (!new UpdateHelloValidator().IsValid(request, out var errors))
                return new ValidationErrorResult(errors);

            if (!await _helloRepo.IsSameRowVersion(request.Id!.Value, request.RowVersion!))
                return new Error<string>("Item has already been updated by other user.");

            var text = "Hello " + request.Input;

            var res = await _helloRepo.UpdateAsync(request.Id!.Value, text, request.RowVersion!, "User_1");

            return new Result<SqlResult?>(res);
        }
    }
}