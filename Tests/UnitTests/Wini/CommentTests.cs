namespace Tests.UnitTests.Wini;

public class CommentTests
{
    [Fact]
    public void Add_New_Comment()
    {
        var createdDate = new DateTime(2024, 1, 1);
        var booking = CreateNewBookingAndComment(GetAuthService().Object, createdDate);

        Assert.Single(booking.Comments);
        Assert.Single(booking.DomainEvents);
        Assert.Contains(booking.Comments, _ => _.Value == "Test");
        Assert.All(booking.DomainEvents, _ => Assert.IsType<CommentActionEvent>(_));
    }

    [Fact]
    public void Edit_Comment()
    {
        var createdDate = new DateTime(2024, 1, 1);
        var authService = GetAuthService();
        var booking = CreateNewBookingAndComment(authService.Object, createdDate);

        var commentToEdit = booking.Comments[0];
        booking.EditComment("Test edit", commentToEdit.Created, authService.Object);

        Assert.Single(booking.Comments);
        Assert.Contains(booking.Comments, _ => _.Value == "Test edit");
        Assert.Equal(2, booking.DomainEvents.Count);
        var firstEvent = booking.DomainEvents[0] as CommentActionEvent;
        Assert.NotNull(firstEvent);
        Assert.Equal(CrudAction.Added, firstEvent.Action);
        Assert.Equal("Test", firstEvent.Comment!.Value);

        var secondEvent = booking.DomainEvents[1] as CommentActionEvent;
        Assert.NotNull(secondEvent);
        Assert.Equal(CrudAction.Edited, secondEvent.Action);
        Assert.Equal(booking.Comments[0], secondEvent.Comment);
    }

    [Fact]
    public void Delete_Comment()
    {
        var authService = GetAuthService();
        var createdDate = new DateTime(2024, 1, 1);

        var booking = CreateNewBookingAndComment(authService.Object, createdDate);
        var commentToDelete = booking.Comments[0];

        booking.DeleteComment(commentToDelete.Created, authService.Object);

        Assert.Empty(booking.Comments);
        Assert.Equal(2, booking.DomainEvents.Count);

        var secondEvent = booking.DomainEvents[1] as CommentActionEvent;
        Assert.NotNull(secondEvent);
        Assert.Equal(CrudAction.Deleted, secondEvent.Action);
        Assert.Equal(commentToDelete, secondEvent.Comment);
    }

    [Fact]
    public void Fail_To_Add_New_Comment_Not_Commissioner()
    {
        var authService = GetAuthService("XMIHST");
        var createdDate = new DateTime(2024, 1, 1);

        var ex = Assert.Throws<DomainLogicException>(() => CreateNewBookingAndComment(authService.Object, createdDate));

        Assert.NotNull(ex);
        Assert.Equal("Only commissioners can add comments.", ex.Message);
    }

    [Fact]
    public void Fail_To_Add_New_Comment_Booking_Not_Saved()
    {
        var authService = GetAuthService("MIHSTE");
        var booking = new Booking(default, new Commissioner(authService.Object.GetUserId()));
        var createdDate = new DateTime(2024, 1, 1);
        var ex = Assert.Throws<DomainLogicException>(() => booking.AddNewComment("Test", createdDate, authService.Object));

        Assert.NotNull(ex);
        Assert.Equal("Cannot add comment on unsaved bookings.", ex.Message);
    }

    private static Booking CreateNewBookingAndComment(
        IAuthenticationService authenticationService,
        DateTime createdDate,
        string commissioner = "MIHSTE",
        string comment = "Test"
    )
    {
        var booking = new Booking(new IdValue<int>(1), new Commissioner(commissioner));

        booking.AddNewComment(comment, createdDate, authenticationService);

        return booking;
    }

    private static Mock<IAuthenticationService> GetAuthService(string userId = "MIHSTE")
    {
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(userId);
        return authenticationService;
    }
}