namespace Tests.ApiTests.Wini;

[Order(4)]
public sealed class CommentApiTests : IClassFixture<TestBase>
{
    private readonly TestBase _testBase;

    public CommentApiTests(TestBase testBase)
    {
        _testBase = testBase;
    }

    [Fact]
    public async Task Insert_New_Comment()
    {
        await _testBase.ResetDbAsync();
        var sqlResult = await _testBase.SeedBaseBookingAsync(default, default);
        var command = new UpdateBookingCommentCommand
        {
            RowVersion = sqlResult.RowVersion,
            Value = "TEST",
            Action = CrudAction.Added,
            Created = DateTime.UtcNow
        };

        var res = await _testBase.HttpClient.PatchAsJsonAsync($"/api/booking/{sqlResult.Id}/comment", command);
        Assert.Equal(System.Net.HttpStatusCode.OK, res.StatusCode);

        var content = await res.Content.ReadFromJsonAsync<SqlResult>();
        Assert.NotNull(content);

        var comments = await _testBase.QueryAsync<Services.DatabaseDapper.Models.Comment>("SELECT * FROM dbo.Comments WHERE BookingId = @BookingId", new { BookingId = sqlResult.Id });
        Assert.Single(comments);
        Assert.Equal(command.Value, comments.FirstOrDefault()?.Value);
    }

    [Fact]
    public async Task Update_Comment()
    {
        await _testBase.ResetDbAsync();
        var createdDate = DateTime.UtcNow;
        var comments = new[] {
            new Services.DatabaseDapper.Models.Comment {
                Created = createdDate,
                CreatedBy = "MIHSTE",
                Value = "ASDFG"
            },
            new Services.DatabaseDapper.Models.Comment {
                Created = new DateTime(2024, 1, 1),
                CreatedBy = "MIHSTE",
                Value = "XYZ"
            }
        };
        var sqlResult = await _testBase.SeedBaseBookingAsync(default, default, comments);

        var command = new UpdateBookingCommentCommand
        {
            RowVersion = sqlResult.RowVersion,
            Value = "TEST",
            Created = createdDate,
            Action = CrudAction.Edited
        };

        var res = await _testBase.HttpClient.PatchAsJsonAsync($"/api/booking/{sqlResult.Id}/comment", command);
        Assert.Equal(System.Net.HttpStatusCode.OK, res.StatusCode);

        var content = await res.Content.ReadFromJsonAsync<SqlResult>();
        Assert.NotNull(content);

        var dbComments = await _testBase.QueryAsync<Services.DatabaseDapper.Models.Comment>("SELECT * FROM dbo.Comments WHERE BookingId = @BookingId ORDER BY Created DESC", new { BookingId = sqlResult.Id });
        Assert.Equal(2, dbComments.Count());
        Assert.Equal("TEST", dbComments.FirstOrDefault()?.Value);
        Assert.Equal("XYZ", dbComments.LastOrDefault()?.Value);
    }

    [Fact]
    public async Task Delete_Comment()
    {
        await _testBase.ResetDbAsync();
        var createdDate = DateTime.UtcNow;
        var comments = new[] {
            new Services.DatabaseDapper.Models.Comment {
                Created = createdDate,
                CreatedBy = "MIHSTE",
                Value = "ASDFG"
            },
            new Services.DatabaseDapper.Models.Comment {
                Created = new DateTime(2024, 1, 1),
                CreatedBy = "MIHSTE",
                Value = "XYZ"
            }
        };
        var sqlResult = await _testBase.SeedBaseBookingAsync(default, default, comments);

        var command = new UpdateBookingCommentCommand
        {
            RowVersion = sqlResult.RowVersion,
            Created = createdDate,
            Action = CrudAction.Deleted
        };

        var res = await _testBase.HttpClient.PatchAsJsonAsync($"/api/booking/{sqlResult.Id}/comment", command);
        Assert.Equal(System.Net.HttpStatusCode.OK, res.StatusCode);

        var content = await res.Content.ReadFromJsonAsync<SqlResult>();
        Assert.NotNull(content);

        var dbComments = await _testBase.QueryAsync<Services.DatabaseDapper.Models.Comment>("SELECT * FROM dbo.Comments WHERE BookingId = @BookingId ORDER BY Created DESC", new { BookingId = sqlResult.Id });
        Assert.Single(dbComments);
        Assert.Equal("XYZ", dbComments.LastOrDefault()?.Value);
    }
}