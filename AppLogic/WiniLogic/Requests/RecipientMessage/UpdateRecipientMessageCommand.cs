namespace AppLogic.WiniLogic.Requests;

public class UpdateRecipientMessageCommand : IRequest<OneOf<Result<SqlResult>, ValidationErrorResult, Error<string>, NotFound, ForbiddenResult, Unknown>>
{
    public int? BookingId { get; set; }
    public CrudAction? Action { get; set; }
    public string? Value { get; set; }
    public string? Recipient { get; set; }

    public class UpdateRecipientMessageValidator : AbstractValidator<UpdateRecipientMessageCommand>
    {
        public UpdateRecipientMessageValidator()
        {
            RuleFor(_ => _.Recipient).NotEmpty();
            RuleFor(_ => _.Action).NotEmpty();
            When(_ => _.Action != CrudAction.Deleted, () => RuleFor(_ => _.Value).NotEmpty());
            RuleFor(_ => _.BookingId).NotEmpty().GreaterThan(0);
        }
    }

    public class UpdateRecipientMessageHandler(
        IBookingRepository bookingRepo,
        IAuthenticationService authenticationService,
        IAuthorizationService authorizationService,
        ITransactionScopeManager transactionManager,
        ILogger<UpdateRecipientMessageHandler> logger
    )
    : IRequestHandler<UpdateRecipientMessageCommand, OneOf<Result<SqlResult>, ValidationErrorResult, Error<string>, NotFound, ForbiddenResult, Unknown>>
    {
        public async Task<OneOf<Result<SqlResult>, ValidationErrorResult, Error<string>, NotFound, ForbiddenResult, Unknown>> Handle(
            UpdateRecipientMessageCommand request,
            CancellationToken cancellationToken)
        {
            if (!authorizationService.IsWrite())
            {
                return new ForbiddenResult();
            }

            if (!new UpdateRecipientMessageValidator().IsValid(request, out var requestErrors))
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

                UpdateRecipientMessageByAction(request.Action, res.Value.Booking, request.Recipient!, request.Value!);

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
                logger.LogError("An database error occurred when trying to edit booking recipient. Message = {message}", ex.Message);
                return new Unknown();
            }
        }

        private void UpdateRecipientMessageByAction(CrudAction? action, Booking booking, string recipient, string value)
        {
            switch (action)
            {
                case CrudAction.Added:
                    booking.AddNewRecipientMessage(value, recipient, authenticationService);
                    break;
                case CrudAction.Edited:
                    booking.EditRecipientMessage(value, recipient, authenticationService);
                    break;
                case CrudAction.Deleted:
                    booking.DeleteRecipientMessage(recipient, authenticationService);
                    break;
            }
        }
    }
}