namespace AppLogic.Requests;

public class UpdateBookingCommand : BookingInput, IRequest<OneOf<Result<SqlResult>, ValidationErrorResult, ConflictResult, Error<string>, ForbiddenResult, NotFound, Unknown>>
{
    public int? BookingId { get; set; }
    public int[] RowNumbersToDelete { get; set; } = Array.Empty<int>();
    public byte[]? RowVersion { get; set; }

    public class UpdateBookingValidator : AbstractValidator<UpdateBookingCommand>
    {
        public UpdateBookingValidator()
        {
            RuleFor(_ => _.BookingId).GreaterThan(0);
            RuleFor(_ => _.BookingDate).GreaterThan(new DateTime(2000, 1, 1));
            RuleFor(_ => _.RowVersion).NotEmpty();
            RuleFor(_ => _.RowNumbersToDelete).NotNull();
        }
    }

    public class UpdateBookingHandler
    : IRequestHandler<UpdateBookingCommand, OneOf<Result<SqlResult>, ValidationErrorResult, ConflictResult, Error<string>, ForbiddenResult, NotFound, Unknown>>
    {
        private readonly IBookingRepository _repo;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ITransactionScopeManager _transactionManager;
        private readonly ILogger<UpdateBookingHandler> _logger;

        public UpdateBookingHandler(
            IBookingRepository repo,
            IAuthenticationService authenticationService,
            IAuthorizationService authorizationService,
            ITransactionScopeManager transactionManager,
            ILogger<UpdateBookingHandler> logger
        )
        {
            _repo = repo;
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
            _transactionManager = transactionManager;
            _logger = logger;
        }

        public async Task<OneOf<Result<SqlResult>, ValidationErrorResult, ConflictResult, Error<string>, ForbiddenResult, NotFound, Unknown>> Handle(UpdateBookingCommand request, CancellationToken cancellationToken)
        {
            if (!_authorizationService.IsWrite())
                return new ForbiddenResult();

            if (!new UpdateBookingValidator().IsValid(request, out var errors))
                return new ValidationErrorResult(errors);

            try
            {
                if (!await _repo.IsSameRowVersionAsync(request.BookingId!.Value, request.RowVersion!))
                    return new ConflictResult();

                var result = await _repo.GetBookingIdAsync(request.BookingId);
                if (result == default) return new NotFound();
                var booking = result.Value.Booking;

                booking.EditBookingHeader(
                    new BookingHeaderModel(
                        request.BookingDate ?? booking.Header.BookingDate.Date,
                        request.TextToE1 ?? booking.Header.TextToE1.Text,
                        request.IsReversed ?? booking.Header.IsReversed,
                        request.LedgerType ?? booking.Header.LedgerType.Type
                    ), _authenticationService
                );

                if (request.RowNumbersToDelete.Length > 0)
                    booking.DeleteMultipleRows(request.RowNumbersToDelete, _authenticationService);

                if (request.Rows.Length > 0)
                    booking.UpsertMultipleRows(request.Rows, _authenticationService);

                using var transaction = _transactionManager.CreateTransaction();
                var user = _authenticationService.GetUserId();

                var res = await _repo.UpdateBookingAsync(booking, user);
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
            catch (Exception ex) when (ex is DatabaseOperationException or InvalidOperationException or DatabaseValueException)
            {
                _logger.LogError("An database error occurred when trying to edit booking. Message = {message}", ex.Message);
                return new Unknown();
            }
        }
    }
}