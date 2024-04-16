namespace AppLogig.Requests;

public class GetAttachmentByNameQuery
: IRequest<OneOf<Result<AttachmentDto>, ValidationErrorResult, NotFound, ForbiddenResult, Unknown>>
{
    public int? BookingId { get; set; }
    public string? FileName { get; set; }
    public class GetAttachmentByNameValidator : AbstractValidator<GetAttachmentByNameQuery>
    {
        public GetAttachmentByNameValidator()
        {
            RuleFor(_ => _.BookingId).NotEmpty();
            RuleFor(_ => _.FileName).MaximumLength(300).NotEmpty();
        }
    }

    public class GetAttachmentByNameHandler(
        IBookingRepository bookingRepository,
        IAuthorizationService authorizationService
    )
    : IRequestHandler<GetAttachmentByNameQuery, OneOf<Result<AttachmentDto>, ValidationErrorResult, NotFound, ForbiddenResult, Unknown>>
    {
        public async Task<OneOf<Result<AttachmentDto>, ValidationErrorResult, NotFound, ForbiddenResult, Unknown>> Handle(
            GetAttachmentByNameQuery request,
            CancellationToken cancellationToken
        )
        {
            if (!authorizationService.IsRead())
            {
                return new ForbiddenResult();
            }

            if (!new GetAttachmentByNameValidator().IsValid(request, out var requestErrors))
            {
                return new ValidationErrorResult(requestErrors);
            }

            var attachment = await bookingRepository.GetAttachmentAsync(request.BookingId!.Value, request.FileName!);
            if (attachment == null)
            {
                return new NotFound();
            }

            var dto = new AttachmentDto(
                Name: attachment.Content.Name,
                ContentType: attachment.Content.ContentType,
                Size: attachment.Content.Size,
                Path: attachment.Content.Path
            );

            return new Result<AttachmentDto>(dto);
        }
    }
}
