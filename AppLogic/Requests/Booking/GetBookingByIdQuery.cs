namespace AppLogic.Requests;

public class GetBookingByIdQuery : IRequest<OneOf<Result<BookingDto>, ValidationErrorResult, ForbiddenResult, NotFound>>
{
    public int? BookingId { get; set; }

    public class GetBookingByIdValidator : AbstractValidator<GetBookingByIdQuery>
    {
        public GetBookingByIdValidator()
        {
            RuleFor(_ => _.BookingId).NotEmpty().GreaterThan(0);
        }
    }

    public class GetBookingByIdHandler(
        IBookingRepository repo,
        IAuthorizationService authorizationService)
    : IRequestHandler<GetBookingByIdQuery, OneOf<Result<BookingDto>, ValidationErrorResult, ForbiddenResult, NotFound>>
    {
        public async Task<OneOf<Result<BookingDto>, ValidationErrorResult, ForbiddenResult, NotFound>> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
        {
            if (!authorizationService.IsRead())
            {
                return new ForbiddenResult();
            }

            if (!new GetBookingByIdValidator().IsValid(request, out var errors))
            {
                return new ValidationErrorResult(errors);
            }

            var result = await repo.GetBookingIdAsync(request.BookingId);
            if (result == default)
            {
                return new NotFound();
            }

            var booking = result.Value.Booking;

            var dto = new BookingDto(
                BookingId: booking.BookingId!.Value,
                Status: booking.BookingStatus.Status,
                BookingDate: booking.Header.BookingDate.Date.ToString("yyyy-MM-dd"),
                TextToE1: booking.Header.TextToE1.Text,
                Reversed: booking.Header.IsReversed,
                LedgerType: booking.Header.LedgerType.Type,
                Commissioner: booking.Commissioner.UserId ?? throw new NullReferenceException("Commissioner cannot be null."),
                UpdatedBy: booking.BookingStatus.UpdatedBy.UserId!,
                Updated: booking.BookingStatus.Updated,
                RowVersion: result.Value.RowVersion,
                Rows: booking.Rows.Select(_ => new BookingRowDto(
                    RowNumber: _.RowNumber,
                    BusinessUnit: _.BusinessUnit.ToString(),
                    Account: _.Account.Value,
                    Subsidiary: _.Account.Subsidiary,
                    Subledger: _.Subledger.Value,
                    SubledgerType: _.Subledger.Type,
                    CostObject1: _.CostObject1.Value,
                    CostObjectType1: _.CostObject1.Type,
                    CostObject2: _.CostObject2.Value,
                    CostObjectType2: _.CostObject2.Type,
                    CostObject3: _.CostObject3.Value,
                    CostObjectType3: _.CostObject3.Type,
                    CostObject4: _.CostObject4.Value,
                    CostObjectType4: _.CostObject4.Type,
                    Remark: _.Remark.Text,
                    Authorizer: _.Authorizer.UserId,
                    IsAuthorized: _.Authorizer.HasAuthorized,
                    Amount: _.Money.Amount,
                    CurrencyCode: _.Money.Currency.CurrencyCode.Code,
                    ExchangeRate: _.Money.Currency.ExchangeRate
                )),
                StatusHistory: booking.BookingStatus.StatusHistory.Select(_ => new StatusDto(
                    Status: _.Status,
                    Updated: _.Updated,
                    UpdatedBy: _.UpdatedBy.UserId!
                )),
                Comments: booking.Comments.Select(_ => new CommentDto(
                    Value: _.Value ?? "",
                    Created: _.Created
                )),
                Messages: booking.Messages.Select(_ => new RecipientMessageDto(
                    Recipient: _.Recipient.UserId ?? "N/A",
                    Message: _.Message
                )),
                Attachments: booking.Attachments.Select(_ => new AttachmentDto(
                    Size: _.Content.Size,
                    ContentType: _.Content.ContentType,
                    Path: _.Content.Path,
                    Name: _.Content.Name
                )),
                DeletedRowNumbers: result.Value.DeletedRows,
                MaxRowNumber: booking.Rows.Select(_ => _.RowNumber).Concat(result.Value.DeletedRows ?? []).Max()
            );
            return new Result<BookingDto>(dto);
        }
    }
}