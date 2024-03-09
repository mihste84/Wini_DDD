using System.Web;

namespace Tests.ApiTests.Wini;

[Order(3)]
public sealed class CommentApiTests(TestBase testBase) : IClassFixture<TestBase>
{
    private readonly TestBase _testBase = testBase;

    [Fact]
    public async Task Insert_New_Comment_Async()
    {
        await _testBase.ResetDbAsync();
        var sqlResult = await _testBase.SeedBaseBookingAsync(default, default);
        var command = new CommentInput(DateTime.UtcNow, "TEST", sqlResult.RowVersion);

        var res = await _testBase.HttpClient.PostAsJsonAsync($"/api/booking/{sqlResult.Id}/comment", command);
        Assert.Equal(System.Net.HttpStatusCode.Created, res.StatusCode);

        var content = await res.Content.ReadFromJsonAsync<SqlResult>();
        Assert.NotNull(content);

        var comments = await _testBase.QueryAsync<Services.DatabaseDapper.Models.Comment>(
            "SELECT * FROM dbo.Comments WHERE BookingId = @BookingId",
            new { BookingId = sqlResult.Id }
        );
        Assert.Single(comments);
        Assert.Equal(command.Value, comments.FirstOrDefault()?.Value);
    }

    [Fact]
    public async Task Update_Comment_Async()
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
                Created = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "MIHSTE",
                Value = "XYZ"
            }
        };
        var sqlResult = await _testBase.SeedBaseBookingAsync(default, default, comments);
        var command = new CommentInput(DateTime.UtcNow, "TEST", sqlResult.RowVersion);

        var res = await _testBase.HttpClient.PatchAsJsonAsync($"/api/booking/{sqlResult.Id}/comment", command);
        Assert.Equal(System.Net.HttpStatusCode.OK, res.StatusCode);

        var content = await res.Content.ReadFromJsonAsync<SqlResult>();
        Assert.NotNull(content);

        var dbComments = await _testBase.QueryAsync<Services.DatabaseDapper.Models.Comment>(
            "SELECT * FROM dbo.Comments WHERE BookingId = @BookingId ORDER BY Created DESC",
            new { BookingId = sqlResult.Id }
        );
        Assert.Equal(2, dbComments.Count());
        Assert.Equal("TEST", dbComments.FirstOrDefault()?.Value);
        Assert.Equal("XYZ", dbComments.LastOrDefault()?.Value);
    }

    [Fact]
    public async Task Delete_Comment_Async()
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
                Created = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Local),
                CreatedBy = "MIHSTE",
                Value = "XYZ"
            }
        };
        var sqlResult = await _testBase.SeedBaseBookingAsync(default, default, comments);
        var rowVersion = "rowVersion=" + string.Join("&rowVersion=", sqlResult.RowVersion!);
        var res = await _testBase.HttpClient.DeleteAsync(
            $"/api/booking/{sqlResult.Id}/comment?{rowVersion}&created={HttpUtility.UrlEncode(createdDate.ToString("yyyy-MM-dd HH:mm:ss.fffffff"))}"
        );
        Assert.Equal(System.Net.HttpStatusCode.OK, res.StatusCode);

        var content = await res.Content.ReadFromJsonAsync<SqlResult>();
        Assert.NotNull(content);

        var dbComments = await _testBase.QueryAsync<Services.DatabaseDapper.Models.Comment>(
            "SELECT * FROM dbo.Comments WHERE BookingId = @BookingId ORDER BY Created DESC",
            new { BookingId = sqlResult.Id }
        );
        Assert.Single(dbComments);
        Assert.Equal("XYZ", dbComments.LastOrDefault()?.Value);
    }
}