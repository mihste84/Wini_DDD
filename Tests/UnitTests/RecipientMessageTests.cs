namespace Tests.UnitTests;

public class RecipientMessageTests
{
    private readonly IdValue<int> _bookingId = new(1);
    [Fact]
    public void Create_RecipientMessage()
    {
        const string message = "testing message";
        var user = new User("MIHSTE");
        var rm = new RecipientMessage(message, user, _bookingId);
        Assert.Equal(message, rm.Message);
        Assert.Equal(user.UserId, rm.Recipient.UserId);
    }

    [Fact]
    public void Fail_To_Create_RecipientMessage_With_Missing_Message()
    {
        const string message = "";
        var user = new User("MIHSTE");

        var ex = Assert.Throws<DomainLogicException>(() => new RecipientMessage(message, user, _bookingId));

        Assert.Equal("message", ex.PropertyName);
        Assert.Equal(ValidationErrorCodes.Required, ex.ErrorCode);
        Assert.Equal("Message cannot be empty when recipient is set", ex.Message);
    }

    [Fact]
    public void Fail_To_Create_RecipientMessage_With_Too_Long_Message()
    {
        var message = new string('X', 101);
        var user = new User("MIHSTE");

        var ex = Assert.Throws<TextValidationException>(() => new RecipientMessage(message, user, _bookingId));

        Assert.Equal("message", ex.PropertyName);
        Assert.Equal(message, ex.AttemptedValue);
        Assert.Equal(ValidationErrorCodes.TextTooLong, ex.ErrorCode);
        Assert.Equal("Message cannot be longer than 100 characters", ex.Message);
    }
}