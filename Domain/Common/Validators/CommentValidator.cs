namespace Domain.Common.Validators;
public class CommentValidator : AbstractValidator<Comment>
{
    public CommentValidator()
    {
        RuleFor(_ => _.BookingId).NotNull();
        RuleFor(_ => _.Created).NotNull();
        RuleFor(_ => _.Value).MaximumLength(300).WithName("Comment");
    }
}