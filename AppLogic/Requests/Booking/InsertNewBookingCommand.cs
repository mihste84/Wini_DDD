
namespace AppLogic.Requests;

public class InsertNewBookingCommand : BookingInput, IRequest<OneOf<Result<SqlResult>, ValidationErrorResult, Error<string>, ForbiddenResult, Unknown>>
{
    public class InsertNewBookingValidator : AbstractValidator<InsertNewBookingCommand>
    {
        public InsertNewBookingValidator()
        {
            RuleFor(_ => _.BookingDate).GreaterThan(new DateTime(2000, 1, 1));
            RuleFor(_ => _.Rows).Must(_ => _?.Length <= 1000).WithMessage("Number of rows cannot be greater than 1000.");
        }
    }

    public class InsertNewBookingHandler : IRequestHandler<InsertNewBookingCommand, OneOf<Result<SqlResult>, ValidationErrorResult, Error<string>, ForbiddenResult, Unknown>>
    {
        private readonly IBookingRepository _repo;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IDateTimeService _dateTimeService;
        private readonly ITransactionScopeManager _transactionManager;
        private readonly ILogger<InsertNewBookingHandler> _logger;
        public InsertNewBookingHandler(
            IBookingRepository repo,
            IAuthenticationService authenticationService,
            IAuthorizationService authorizationService,
            IDateTimeService dateTimeService,
            ILogger<InsertNewBookingHandler> logger,
            ITransactionScopeManager transactionFactory
        )
        {
            _repo = repo;
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
            _dateTimeService = dateTimeService;
            _transactionManager = transactionFactory;
            _logger = logger;
        }

        public async Task<OneOf<Result<SqlResult>, ValidationErrorResult, Error<string>, ForbiddenResult, Unknown>> Handle(InsertNewBookingCommand request, CancellationToken cancellationToken)
        {
            if (!_authorizationService.IsWrite())
                return new ForbiddenResult();

            var user = _authenticationService.GetUserId();
            var now = _dateTimeService.GetDateTimeUtc();

            try
            {
                var booking = new Booking(
                    default,
                    new BookingStatus(WiniStatus.Saved, now, user),
                    new Commissioner(user),
                    new BookingDate(request.BookingDate ?? now),
                    new TextToE1(request.TextToE1 ?? ""),
                    request.IsReversed ?? false,
                    new LedgerType(request.LedgerType ?? Ledgers.AA)
                );

                booking.AddMultipleRows(request.Rows, _authenticationService);

                using var transaction = _transactionManager.CreateTransaction();

                var res = await _repo.InsertBookingAsync(booking, user);

                transaction.Complete();

                return res != default
                    ? new Result<SqlResult>(res)
                    : new Unknown();
            }
            catch (DomainValidationException ex)
            {
                return new ValidationErrorResult(ex.Errors);
            }
            catch (Exception ex) when (ex is DomainLogicException or NotFoundException)
            {
                return new Error<string>(ex.Message);
            }
            catch (Exception ex) when (ex is DatabaseOperationException or InvalidOperationException)
            {
                _logger.LogError("An database error occurred when trying to insert booking. Message = {message}", ex.Message);
                return new Unknown();
            }
        }
    }
}