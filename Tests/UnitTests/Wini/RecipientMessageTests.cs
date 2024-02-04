namespace Tests.UnitTests.Wini;

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

        var ex = Assert.Throws<DomainValidationException>(() => new RecipientMessage(message, user, _bookingId));

        Assert.Single(ex.Errors);
        var error = ex.Errors.First();
        Assert.Equal(message, error.AttemptedValue);
        Assert.Equal("NotEmptyValidator", error.ErrorCode);
        Assert.Equal("Message", error.PropertyName);
        Assert.Equal("'Message' must not be empty.", error.Message);
    }

    [Fact]
    public void Fail_To_Create_RecipientMessage_With_Too_Long_Message()
    {
        var message = new string('X', 301);
        var user = new User("MIHSTE");

        var ex = Assert.Throws<DomainValidationException>(() => new RecipientMessage(message, user, _bookingId));

        Assert.Single(ex.Errors);
        var error = ex.Errors.First();
        Assert.Equal(message, error.AttemptedValue);
        Assert.Equal("MaximumLengthValidator", error.ErrorCode);
        Assert.Equal("Message", error.PropertyName);
        Assert.Equal("The length of 'Message' must be 300 characters or fewer. You entered 301 characters.", error.Message);
    }
}