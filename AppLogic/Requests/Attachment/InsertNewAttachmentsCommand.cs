
namespace AppLogic.Requests;

public class InsertAttachmentsCommand : IRequest<OneOf<Result<SqlResult>, ValidationErrorResult, Error<string>, NotFound, ForbiddenResult, Unknown>>
{
    public const int MaxFileSize = 1024 * 1024 * 5; // 5 MB
    public int? BookingId { get; set; }
    public byte[]? RowVersion { get; set; }
    public IEnumerable<UploadedAttachmentInput>? Files { get; set; }
    public class InsertAttachmentsValidator : AbstractValidator<InsertAttachmentsCommand>
    {
        public InsertAttachmentsValidator()
        {
            RuleFor(_ => _.BookingId).NotEmpty();
            RuleFor(_ => _.Files).NotEmpty();
            RuleFor(_ => _.Files)
                .NotEmpty()
                .Must(_ => _?.Count() <= 5)
                .WithMessage("Cannot upload more than 5 files.");

            RuleForEach(_ => _.Files).SetValidator(new UploadedAttachmentInputValidator());
        }
    }

    public class UploadedAttachmentInputValidator : AbstractValidator<UploadedAttachmentInput>
    {
        public UploadedAttachmentInputValidator()
        {
            RuleFor(_ => _.Path).NotEmpty();
            RuleFor(_ => _.ContentType).NotEmpty();
            RuleFor(_ => _.FileName).MaximumLength(300).NotEmpty();
            RuleFor(_ => _.Length)
                .NotEmpty()
                .LessThanOrEqualTo(MaxFileSize)
                .WithMessage(_ => $"File {_.FileName} has size greater than 5MB."); // 5 MB
        }
    }

    public class InsertAttachmentsHandler(
        IBookingRepository bookingRepo,
        IAuthenticationService authenticationService,
        IAuthorizationService authorizationService,
        ITransactionScopeManager transactionManager,
        ILogger<InsertAttachmentsHandler> logger
    )
    : IRequestHandler<InsertAttachmentsCommand, OneOf<Result<SqlResult>, ValidationErrorResult, Error<string>, NotFound, ForbiddenResult, Unknown>>
    {
        public async Task<OneOf<Result<SqlResult>, ValidationErrorResult, Error<string>, NotFound, ForbiddenResult, Unknown>> Handle(
            InsertAttachmentsCommand request,
            CancellationToken cancellationToken
        )
        {
            if (!authorizationService.IsWrite())
            {
                return new ForbiddenResult();
            }

            if (!new InsertAttachmentsValidator().IsValid(request, out var requestErrors))
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

                foreach (var file in request.Files!)
                {
                    booking.AddNewAttachment(
                        file.FileName!,
                        file.ContentType!,
                        file.Path!,
                        file.Length!.Value,
                        authenticationService
                    );
                }

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
            catch (AttachmentServiceException ex)
            {
                var fileName = ex.Input?.FileName ?? "unknown";
                logger.LogWarning("Failed to save file with name '{fileName}' and booking ID {BookingId}.", fileName, request.BookingId);
                return new Error<string>($"Failed to save file with name '{fileName}'.");
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