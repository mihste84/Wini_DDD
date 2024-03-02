namespace AppLogic.Requests;

public class UpdateBookingCommentCommand : IRequest<OneOf<Result<SqlResult>, ValidationErrorResult, ConflictResult, Error<string>, NotFound, ForbiddenResult, Unknown>>
{
    public int? BookingId { get; set; }
    public DateTime? Created { get; set; }
    public CrudAction? Action { get; set; }
    public string? Value { get; set; }
    public byte[]? RowVersion { get; set; }

    public class UpdateBookingCommentValidator : AbstractValidator<UpdateBookingCommentCommand>
    {
        public UpdateBookingCommentValidator()
        {
            RuleFor(_ => _.RowVersion).NotEmpty();
            RuleFor(_ => _.Created).NotEmpty();
            RuleFor(_ => _.Action).NotEmpty();
            RuleFor(_ => _.BookingId).NotEmpty().GreaterThan(0);
        }
    }

    public class UpdateBookingCommentHandler
        : IRequestHandler<UpdateBookingCommentCommand, OneOf<Result<SqlResult>, ValidationErrorResult, ConflictResult, Error<string>, NotFound, ForbiddenResult, Unknown>>
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ITransactionScopeManager _transactionManager;
        private readonly ILogger<UpdateBookingCommentHandler> _logger;

        public UpdateBookingCommentHandler(
            IBookingRepository bookingRepo,
            IAuthenticationService authenticationService,
            IAuthorizationService authorizationService,
            ITransactionScopeManager transactionManager,
            ILogger<UpdateBookingCommentHandler> logger
        )
        {
            _bookingRepo = bookingRepo;
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
            _transactionManager = transactionManager;
            _logger = logger;
        }

        public async Task<OneOf<Result<SqlResult>, ValidationErrorResult, ConflictResult, Error<string>, NotFound, ForbiddenResult, Unknown>> Handle(UpdateBookingCommentCommand request, CancellationToken cancellationToken)
        {
            if (!_authorizationService.IsWrite())
                return new ForbiddenResult();

            if (!new UpdateBookingCommentValidator().IsValid(request, out var requestErrors))
                return new ValidationErrorResult(requestErrors);

            try
            {
                var res = await _bookingRepo.GetBookingIdAsync(request.BookingId, false);
                if (res == default)
                    return new NotFound();

                if (!res.Value.RowVersion.SequenceEqual(request.RowVersion!))
                    return new ConflictResult();

                UpdateCommentByAction(request.Action, res.Value.Booking, request.Created!.Value, request.Value);

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
                _logger.LogError("An database error occurred when trying to edit booking comment. Message = {message}", ex.Message);
                return new Unknown();
            }
        }

        private void UpdateCommentByAction(CrudAction? action, Booking booking, DateTime createdDate, string? value)
        {
            switch (action)
            {
                case CrudAction.Added: booking.AddNewComment(value, createdDate, _authenticationService); break;
                case CrudAction.Edited: booking.EditComment(value, createdDate, _authenticationService); break;
                case CrudAction.Deleted: booking.DeleteComment(createdDate, _authenticationService); break;
            }
        }
    }
}