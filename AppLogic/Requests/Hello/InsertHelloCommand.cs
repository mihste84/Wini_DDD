
namespace AppLogic.Requests;

public class InsertHelloCommand : IRequest<OneOf<Result<SqlResult?>, ValidationErrorResult>>
{
    public string? Input { get; set; }

    public class InsertHelloValidator : AbstractValidator<InsertHelloCommand>
    {
        public InsertHelloValidator()
        {
            RuleFor(_ => _.Input).NotEmpty().MinimumLength(5);
        }
    }

    public class InsertHelloHandler : IRequestHandler<InsertHelloCommand, OneOf<Result<SqlResult?>, ValidationErrorResult>>
    {
        private readonly IHelloRepository _helloRepo;
        private readonly ITransactionHandler _transaction;

        public InsertHelloHandler(IHelloRepository helloRepo, ITransactionHandler transaction)
        {
            _helloRepo = helloRepo;
            _transaction = transaction;
        }

        public async Task<OneOf<Result<SqlResult?>, ValidationErrorResult>> Handle(InsertHelloCommand request, CancellationToken cancellationToken)
        {
            if (!new InsertHelloValidator().IsValid(request, out var errors))
                return new ValidationErrorResult(errors);

            var text = "Hello " + request.Input!;

            await _transaction.StartAsync();

            await _helloRepo.InsertHelloTextAsync(text + 1, "User_1");

            await _helloRepo.InsertHelloTextAsync(text + 2, "User_2");

            var res = await _helloRepo.InsertHelloTextAsync(text + 3, "User_2");

            await _transaction.CompleteAsync();

            return new Result<SqlResult?>(res);
        }
    }
}