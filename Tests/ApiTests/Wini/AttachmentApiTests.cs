namespace Tests.ApiTests.Wini;

[Order(1)]
public sealed class AttachmentApiTests : IClassFixture<BaseDbTestFixture>, IDisposable
{
    private readonly BaseDbTestFixture _testBase;
    private readonly DatabaseWebApplicationFactory _factory;
    private readonly HttpClient _httpClient;

    public AttachmentApiTests(BaseDbTestFixture testBase)
    {
        _testBase = testBase;
        _factory = new DatabaseWebApplicationFactory(testBase.GetConnectionString());
        _httpClient = _factory.CreateClient();
    }

    [Fact]
    public async Task Insert_New_Attachments_Async()
    {
        await _testBase.ResetDbAsync();
        var sqlResult = await _testBase.SeedBaseBookingAsync(default, default);

        using var formData = new MultipartFormDataContent
        {
            { GetStreamContent("TestFiles/Test1.txt", "text/plain"), "uploadedFiles", "Test1.txt" },
            { GetStreamContent("TestFiles/Test2.txt", "text/plain"), "uploadedFiles", "Test2.txt" }
        };

        var res = await _httpClient.PostAsync($"api/booking/{sqlResult.Id}/attachment", formData);
        Assert.Equal(System.Net.HttpStatusCode.Created, res.StatusCode);
        var content = await res.Content.ReadFromJsonAsync<SqlResult>();
        Assert.NotNull(content);
        var attachments = await _testBase.QueryAsync<Services.DatabaseDapper.Models.Attachment>(
            "SELECT * FROM dbo.Attachments where BookingId = @Id",
            new { content.Id }
        );

        Assert.NotEmpty(attachments);
        Assert.Equal(2, attachments.Count());
        Assert.Contains(attachments, _ => _.ContentType == "text/plain" && _.Name == "Test1.txt");
        Assert.Contains(attachments, _ => _.ContentType == "text/plain" && _.Name == "Test2.txt");
    }


    [Fact]
    public async Task Get_Attachment_Async()
    {
        await _testBase.ResetDbAsync();
        var attachments = new[] {
            new Services.DatabaseDapper.Models.Attachment {
                Created = DateTime.UtcNow,
                CreatedBy = "MIHSTE",
                Name = "Test1.txt",
                ContentType = "text/plain",
                Path = "path/to/file",
                Size = 1234
            },
            new Services.DatabaseDapper.Models.Attachment {
                Created = DateTime.UtcNow,
                CreatedBy = "MIHSTE",
                Name = "Test2.txt",
                ContentType = "text/plain",
                Path = "path/to/file",
                Size = 1234
            },
        };
        var sqlResult = await _testBase.SeedBaseBookingAsync(default, default, default, default, attachments);

        var res = await _httpClient.GetAsync($"api/booking/{sqlResult.Id}/attachment?FileName={attachments[0].Name}");
        Assert.Equal(System.Net.HttpStatusCode.OK, res.StatusCode);

    }


    [Fact]
    public async Task Fail_To_Insert_New_Attachments_File_Too_Large_Async()
    {
        await _testBase.ResetDbAsync();
        var sqlResult = await _testBase.SeedBaseBookingAsync(default, default);

        using var formData = new MultipartFormDataContent
        {
            { GetStreamContent("TestFiles/Test1.txt", "text/plain"), "uploadedFiles", "Test1.txt" },
            { GetStreamContent("TestFiles/Test2.txt", "text/plain"), "uploadedFiles", "Test2.txt" },
            { GetStreamContent("TestFiles/TooLargeFile.txt", "text/plain"), "uploadedFiles", "TooLargeFile.txt" }
        };

        var res = await _httpClient.PostAsync($"api/booking/{sqlResult.Id}/attachment", formData);
        Assert.Equal(System.Net.HttpStatusCode.UnprocessableEntity, res.StatusCode);
        var content = await res.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(content);
        Assert.NotEmpty(content.Extensions);
        Assert.Contains(content.Extensions, _ => _.Value!.ToString()!.Contains("File TooLargeFile.txt has size greater than 5MB."));
    }

    [Fact]
    public async Task Delete_Attachment_Async()
    {
        await _testBase.ResetDbAsync();
        var createdDate = DateTime.UtcNow;
        var attachments = new[] {
            new Services.DatabaseDapper.Models.Attachment {
                Created = createdDate,
                CreatedBy = "MIHSTE",
                Name = "Test1.txt",
                ContentType = "text/plain",
                Path = "path/to/file",
                Size = 1234
            },
            new Services.DatabaseDapper.Models.Attachment {
                Created = createdDate,
                CreatedBy = "MIHSTE",
                Name = "Test2.txt",
                ContentType = "text/plain",
                Path = "path/to/file",
                Size = 1234
            },
        };
        var sqlResult = await _testBase.SeedBaseBookingAsync(default, default, default, default, attachments);

        var res = await _httpClient.DeleteAsync(
            $"api/booking/{sqlResult.Id}/attachment?fileName=Test1.txt"
        );
        Assert.Equal(System.Net.HttpStatusCode.OK, res.StatusCode);

        var content = await res.Content.ReadFromJsonAsync<SqlResult>();
        Assert.NotNull(content);

        var dbAttachments = await _testBase.QueryAsync<Services.DatabaseDapper.Models.Attachment>(
            "SELECT * FROM dbo.Attachments WHERE BookingId = @BookingId ORDER BY Created DESC",
            new { BookingId = sqlResult.Id }
        );

        Assert.Single(dbAttachments);
        Assert.Equal("Test2.txt", dbAttachments.FirstOrDefault()?.Name);
    }

    private static StreamContent GetStreamContent(string path, string mediaType)
    {
        var fileContent = new StreamContent(File.OpenRead(path));
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType);
        return fileContent;
    }

    public void Dispose()
    {
        _factory.Dispose();
    }
}