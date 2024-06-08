namespace Tests.UnitTests.Wini;

public class AttachmentTests
{
    [Fact]
    public void Add_New_Attachment()
    {
        var booking = CreateNewBookingAndAttachment(GetAuthService().Object);

        Assert.Single(booking.Attachments);
        Assert.Contains(
            booking.Attachments,
            _ => _.Content.Name == "Test.csv" &&
                _.Content.Path == "path/to/file" &&
                _.Content.Size == 1024 &&
                _.Content.ContentType == "text/plain"
        );
        Assert.Single(booking.DomainEvents);
        Assert.All(booking.DomainEvents, _ => Assert.IsType<AttachmentActionEvent>(_));
    }

    [Fact]
    public async Task Fail_To_Add_New_Attachment_Duplicate()
    {
        var authenticationService = GetAuthService().Object;
        var booking = CreateNewBookingAndAttachment(authenticationService);

        var ex = await Assert.ThrowsAsync<DomainLogicException>(
            () =>
            {
                booking.AddNewAttachment("Test.csv", "text/plain", "path/to/file", 1024, authenticationService);
                return Task.CompletedTask;
            }
        );

        Assert.Equal("Cannot add attachments with duplicate name.", ex.Message);
    }

    [Fact]
    public async Task Fail_To_Add_New_Attachment_Too_Many()
    {
        var authenticationService = GetAuthService().Object;
        var booking = CreateNewBookingAndAttachment(authenticationService);
        booking.AddNewAttachment("Test1.csv", "text/plain", "path/to/file", 1024, authenticationService);
        booking.AddNewAttachment("Test2.csv", "text/plain", "path/to/file", 1024, authenticationService);
        booking.AddNewAttachment("Test3.csv", "text/plain", "path/to/file", 1024, authenticationService);
        booking.AddNewAttachment("Test4.csv", "text/plain", "path/to/file", 1024, authenticationService);

        var ex = await Assert.ThrowsAsync<DomainLogicException>(
            () =>
            {
                booking.AddNewAttachment("Test5.csv", "text/plain", "path/to/file", 1024, authenticationService);
                return Task.CompletedTask;
            }
        );

        Assert.Equal("Booking cannot contain more than 5 attachments.", ex.Message);
    }

    [Fact]
    public void Delete_Attachment()
    {
        var authenticationService = GetAuthService().Object;
        var booking = CreateNewBookingAndAttachment(authenticationService);
        booking.DeleteAttachment("Test.csv", authenticationService);

        Assert.Empty(booking.Attachments);
        Assert.All(booking.DomainEvents, _ => Assert.IsType<AttachmentActionEvent>(_));
    }

    [Fact]
    public async Task Fail_To_Delete_Attachment_Zero_Attachment()
    {
        var authenticationService = GetAuthService().Object;
        var booking = CreateNewBookingAndAttachment(authenticationService);
        booking.DeleteAttachment("Test.csv", authenticationService);

        var ex = await Assert.ThrowsAsync<DomainLogicException>(
            () =>
            {
                booking.DeleteAttachment("Test.csv", authenticationService);
                return Task.CompletedTask;
            }
        );

        Assert.Equal("Booking does not contain any attachments.", ex.Message);
    }

    [Fact]
    public async Task Fail_To_Delete_Attachment_Wrong_Name()
    {
        var authenticationService = GetAuthService().Object;
        var booking = CreateNewBookingAndAttachment(authenticationService);

        var ex = await Assert.ThrowsAsync<NotFoundException>(
            () =>
            {
                booking.DeleteAttachment("Test1.csv", authenticationService);
                return Task.CompletedTask;
            }
        );

        Assert.Equal("Cannot find attachment to delete.", ex.Message);
    }

    private static Domain.Wini.Aggregates.Booking CreateNewBookingAndAttachment(
        IAuthenticationService authenticationService,
        string commissioner = "MIHSTE",
        string filename = "Test.csv",
        string path = "path/to/file",
        string contentType = "text/plain",
        int size = 1024
    )
    {
        var booking = new Domain.Wini.Aggregates.Booking(new IdValue<int>(1), new Commissioner(commissioner));

        booking.AddNewAttachment(filename, contentType, path, size, authenticationService);

        return booking;
    }

    private static Mock<IAuthenticationService> GetAuthService(string userId = "MIHSTE")
    {
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(userId);
        return authenticationService;
    }
}