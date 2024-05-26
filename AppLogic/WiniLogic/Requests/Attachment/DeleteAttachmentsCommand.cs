
namespace AppLogic.WiniLogic.Requests;

public class DeleteAttachmentCommand : IRequest<OneOf<Result<SqlResult>, ValidationErrorResult, Error<string>, NotFound, ForbiddenResult, Unknown>>
{
    public int? BookingId { get; set; }
    public string? FileName { get; set; }
    public class DeleteAttachmentsValidator : AbstractValidator<DeleteAttachmentCommand>
    {
        public DeleteAttachmentsValidator()
        {
            RuleFor(_ => _.BookingId).NotEmpty();
            RuleFor(_ => _.FileName).NotEmpty().MaximumLength(200);
        }
    }

    public class DeleteAttachmentsHandler(
        IBookingRepository bookingRepo,
        IAuthenticationService authenticationService,
        IAuthorizationService authorizationService,
        ITransactionScopeManager transactionManager,
        ILogger<DeleteAttachmentsHandler> logger
    )
    : IRequestHandler<DeleteAttachmentCommand, OneOf<Result<SqlResult>, ValidationErrorResult, Error<string>, NotFound, ForbiddenResult, Unknown>>
    {
        public async Task<OneOf<Result<SqlResult>, ValidationErrorResult, Error<string>, NotFound, ForbiddenResult, Unknown>> Handle(
            DeleteAttachmentCommand request,
            CancellationToken cancellationToken
        )
        {
            if (!authorizationService.IsWrite())
            {
                return new ForbiddenResult();
            }

            if (!new DeleteAttachmentsValidator().IsValid(request, out var requestErrors))
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

                var booking = res.Value.Booking;

                booking.DeleteAttachment(request.FileName!, authenticationService); // File will get deleted by job

                using var scope = transactionManager.CreateTransaction();

                var sqlResult = await bookingRepo.UpdateBookingAsync(booking, authenticationService.GetUserId());

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
    }
}