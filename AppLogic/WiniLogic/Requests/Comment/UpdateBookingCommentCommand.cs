namespace AppLogic.WiniLogic.Requests;

public class UpdateBookingCommentCommand : IRequest<OneOf<Result<SqlResult>, ValidationErrorResult, Error<string>, NotFound, ForbiddenResult, Unknown>>
{
    public int? BookingId { get; set; }
    public DateTime? Created { get; set; }
    public CrudAction? Action { get; set; }
    public string? Value { get; set; }

    public class UpdateBookingCommentValidator : AbstractValidator<UpdateBookingCommentCommand>
    {
        public UpdateBookingCommentValidator()
        {
            RuleFor(_ => _.Created).NotEmpty();
            RuleFor(_ => _.Action).NotEmpty();
            RuleFor(_ => _.BookingId).NotEmpty().GreaterThan(0);
        }
    }

    public class UpdateBookingCommentHandler(
        IBookingRepository bookingRepo,
        IAuthenticationService authenticationService,
        IAuthorizationService authorizationService,
        ITransactionScopeManager transactionManager,
        ILogger<UpdateBookingCommentHandler> logger
    )
    : IRequestHandler<UpdateBookingCommentCommand, OneOf<Result<SqlResult>, ValidationErrorResult, Error<string>, NotFound, ForbiddenResult, Unknown>>
    {
        public async Task<OneOf<Result<SqlResult>, ValidationErrorResult, Error<string>, NotFound, ForbiddenResult, Unknown>> Handle(
            UpdateBookingCommentCommand request,
            CancellationToken cancellationToken)
        {
            if (!authorizationService.IsWrite())
            {
                return new ForbiddenResult();
            }

            if (!new UpdateBookingCommentValidator().IsValid(request, out var requestErrors))
            {
                return new ValidationErrorResult(requestErrors);
            }

            try
            {
                var res = await bookingRepo.GetBookingIdAsync(request.BookingId, false);
                if (res == default)
                {
                    return new NotFound();
                }

                UpdateCommentByAction(request.Action, res.Value.Booking, request.Created!.Value, request.Value);

                using var scope = transactionManager.CreateTransaction();

                var sqlResult = await bookingRepo.UpdateBookingAsync(res.Value.Booking, authenticationService.GetUserId());

                scope.Complete();

                return sqlResult != default
                    ? new Result<SqlResult>(sqlResult)
                    : new Unknown();
            }
            catch (DomainValidationException ex)
            {
                return new ValidationErrorResult(ex.Errors);
            }
            catch (Exception ex) when (ex is DomainLogicException or NotFoundException or ArgumentException or ArgumentNullException)
            {
                return new Error<string>(ex.Message);
            }
            catch (Exception ex) when (ex is DatabaseOperationException or InvalidOperationException or DatabaseValueException)
            {
                logger.LogError("An database error occurred when trying to edit booking comment. Message = {message}", ex.Message);
                return new Unknown();
            }
        }

        private void UpdateCommentByAction(CrudAction? action, Booking booking, DateTime createdDate, string? value)
        {
            switch (action)
            {
                case CrudAction.Added:
                    booking.AddNewComment(value, createdDate, authenticationService);
                    break;
                case CrudAction.Edited:
                    booking.EditComment(value, createdDate, authenticationService);
                    break;
                case CrudAction.Deleted:
                    booking.DeleteComment(createdDate, authenticationService);
                    break;
            }
        }
    }
}