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

    [Fact]
    public void Add_New_RecipientMessage_To_Booking()
    {
        var createdDate = new DateTime(2024, 1, 1);
        var booking = CreateNewBookingAndRecipientMessage(GetAuthService().Object);

        Assert.Single(booking.Messages);
        Assert.Single(booking.DomainEvents);
        Assert.Contains(booking.Messages, _ => _.Message == "Test" && _.Recipient.UserId == "XMIHST");
        Assert.All(booking.DomainEvents, _ => Assert.IsType<RecipinetMessageActionEvent>(_));
    }

    [Fact]
    public void Edit_RecipientMessage()
    {
        var createdDate = new DateTime(2024, 1, 1);
        var authService = GetAuthService();
        var booking = CreateNewBookingAndRecipientMessage(authService.Object);

        var messageToEdit = booking.Messages[0];
        booking.EditRecipientMessage("Test edit", "XMIHST", authService.Object);

        Assert.Single(booking.Messages);
        Assert.Contains(booking.Messages, _ => _.Message == "Test edit" && _.Recipient.UserId == "XMIHST");
        Assert.Equal(2, booking.DomainEvents.Count);
        var firstEvent = booking.DomainEvents[0] as RecipinetMessageActionEvent;
        Assert.NotNull(firstEvent);
        Assert.Equal(CrudAction.Added, firstEvent.Action);
        Assert.Equal("Test", firstEvent.RecipientMessage!.Message);

        var secondEvent = booking.DomainEvents[1] as RecipinetMessageActionEvent;
        Assert.NotNull(secondEvent);
        Assert.Equal(CrudAction.Edited, secondEvent.Action);
        Assert.Equal(booking.Messages[0], secondEvent.RecipientMessage);
    }

    [Fact]
    public void Delete_RecipientMessage()
    {
        var authService = GetAuthService();

        var booking = CreateNewBookingAndRecipientMessage(authService.Object);
        var messageToDelete = booking.Messages[0];

        booking.DeleteRecipientMessage(messageToDelete.Recipient.UserId!, authService.Object);

        Assert.Empty(booking.Messages);
        Assert.Equal(2, booking.DomainEvents.Count);

        var secondEvent = booking.DomainEvents[1] as RecipinetMessageActionEvent;
        Assert.NotNull(secondEvent);
        Assert.Equal(CrudAction.Deleted, secondEvent.Action);
        Assert.Equal(messageToDelete, secondEvent.RecipientMessage);
    }

    [Fact]
    public void Fail_To_Add_New_RecipientMessage_Not_Commissioner()
    {
        var authService = GetAuthService("XMIHST");
        var createdDate = new DateTime(2024, 1, 1);

        var ex = Assert.Throws<DomainLogicException>(() => CreateNewBookingAndRecipientMessage(authService.Object));

        Assert.NotNull(ex);
        Assert.Equal("Only commissioners can change recipients.", ex.Message);
    }

    [Fact]
    public void Fail_To_Add_New_RecipientMessage_Booking_Not_Saved()
    {
        var authService = GetAuthService("MIHSTE");
        var booking = new Booking(default, new Commissioner(authService.Object.GetUserId()));
        var createdDate = new DateTime(2024, 1, 1);
        var ex = Assert.Throws<DomainLogicException>(() => booking.AddNewRecipientMessage("Test", "XMIHST", authService.Object));

        Assert.NotNull(ex);
        Assert.Equal("Cannot change recipients on unsaved bookings.", ex.Message);
    }

    private static Booking CreateNewBookingAndRecipientMessage(
        IAuthenticationService authenticationService,
        string commissioner = "MIHSTE",
        string recipientMessage = "Test",
        string recipient = "XMIHST"
    )
    {
        var booking = new Booking(new IdValue<int>(1), new Commissioner(commissioner));

        booking.AddNewRecipientMessage(recipientMessage, recipient, authenticationService);

        return booking;
    }

    private static Mock<IAuthenticationService> GetAuthService(string userId = "MIHSTE")
    {
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(userId);
        return authenticationService;
    }
}