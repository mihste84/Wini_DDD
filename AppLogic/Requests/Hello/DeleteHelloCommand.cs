
namespace AppLogic.Requests;

public class DeleteHelloCommand : IRequest<OneOf<Success, No, ValidationErrorResult>>
{
    public int? Id { get; set; }

    public class DeleteHelloValidator : AbstractValidator<DeleteHelloCommand>
    {
        public DeleteHelloValidator()
        {
            RuleFor(_ => _.Id).NotEmpty().GreaterThan(0);
        }
    }

    public class DeleteHelloHandler : IRequestHandler<DeleteHelloCommand, OneOf<Success, No, ValidationErrorResult>>
    {
        private readonly IHelloRepository _helloRepo;

        public DeleteHelloHandler(IHelloRepository helloRepo)
        {
            _helloRepo = helloRepo;
        }

        public async Task<OneOf<Success, No, ValidationErrorResult>> Handle(DeleteHelloCommand request, CancellationToken cancellationToken)
        {
            if (!new DeleteHelloValidator().IsValid(request, out var errors))
                return new ValidationErrorResult(errors);

            var res = await _helloRepo.DeleteByIdAsync(request.Id!.Value);

            return res ? new Success() : new No();
        }
    }
}