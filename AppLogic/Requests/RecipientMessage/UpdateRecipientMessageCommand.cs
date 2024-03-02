namespace AppLogic.Requests;

public class UpdateRecipientMessageCommand : IRequest<OneOf<Result<SqlResult>, ValidationErrorResult, ConflictResult, Error<string>, NotFound, ForbiddenResult, Unknown>>
{
    public int? BookingId { get; set; }
    public CrudAction? Action { get; set; }
    public string? Value { get; set; }
    public string? Recipient { get; set; }
    public byte[]? RowVersion { get; set; }

    public class UpdateRecipientMessageValidator : AbstractValidator<UpdateRecipientMessageCommand>
    {
        public UpdateRecipientMessageValidator()
        {
            RuleFor(_ => _.RowVersion).NotEmpty();
            RuleFor(_ => _.Recipient).NotEmpty();
            RuleFor(_ => _.Action).NotEmpty();
            When(_ => _.Action != CrudAction.Deleted, () => RuleFor(_ => _.Value).NotEmpty());
            RuleFor(_ => _.BookingId).NotEmpty().GreaterThan(0);
        }
    }

    public class UpdateRecipientMessageHandler
        : IRequestHandler<UpdateRecipientMessageCommand, OneOf<Result<SqlResult>, ValidationErrorResult, ConflictResult, Error<string>, NotFound, ForbiddenResult, Unknown>>
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ITransactionScopeManager _transactionManager;
        private readonly ILogger<UpdateRecipientMessageHandler> _logger;

        public UpdateRecipientMessageHandler(
            IBookingRepository bookingRepo,
            IAuthenticationService authenticationService,
            IAuthorizationService authorizationService,
            ITransactionScopeManager transactionManager,
            ILogger<UpdateRecipientMessageHandler> logger
        )
        {
            _bookingRepo = bookingRepo;
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
            _transactionManager = transactionManager;
            _logger = logger;
        }

        public async Task<OneOf<Result<SqlResult>, ValidationErrorResult, ConflictResult, Error<string>, NotFound, ForbiddenResult, Unknown>> Handle(UpdateRecipientMessageCommand request, CancellationToken cancellationToken)
        {
            if (!_authorizationService.IsWrite())
                return new ForbiddenResult();

            if (!new UpdateRecipientMessageValidator().IsValid(request, out var requestErrors))
                return new ValidationErrorResult(requestErrors);

            try
            {
                var res = await _bookingRepo.GetBookingIdAsync(request.BookingId, false);
                if (res == default)
                    return new NotFound();

                if (!res.Value.RowVersion.SequenceEqual(request.RowVersion!))
                    return new ConflictResult();

                UpdateRecipientMessageByAction(request.Action, res.Value.Booking, request.Recipient!, request.Value!);

                using var scope = _transactionManager.CreateTransaction();

                var sqlResult = await _bookingRepo.UpdateBookingAsync(res.Value.Booking, _authenticationService.GetUserId());

                scope.Complete();

                return sqlResult != default
                    ? new Result<SqlResult>(sqlResult)
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
                _logger.LogError("An database error occurred when trying to edit booking recipient. Message = {message}", ex.Message);
                return new Unknown();
            }
        }

        private void UpdateRecipientMessageByAction(CrudAction? action, Booking booking, string recipient, string value)
        {
            switch (action)
            {
                case CrudAction.Added: booking.AddNewRecipientMessage(value, recipient, _authenticationService); break;
                case CrudAction.Edited: booking.EditRecipientMessage(value, recipient, _authenticationService); break;
                case CrudAction.Deleted: booking.DeleteRecipientMessage(recipient, _authenticationService); break;
            }
        }
    }
}