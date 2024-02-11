namespace Tests.ApiTests.Wini;

[Order(1)]
public sealed class HelloWorldApiTests : IClassFixture<TestBase>
{
    private readonly TestBase _testBase;

    public HelloWorldApiTests(TestBase testBase)
    {
        _testBase = testBase;
    }

    [Fact]
    public async Task Insert_Hello_World()
    {
        await _testBase.ResetDbAsync();
        var content = await InsertHelloWorld("World");
        Assert.NotNull(content);
        Assert.IsType<SqlResult>(content);
    }

    [Fact]
    public async Task Insert_Hello_World_Bad_Request()
    {
        await _testBase.ResetDbAsync();
        var res = await _testBase.HttpClient.PostAsJsonAsync("/api/hello", new InsertHelloCommand { Input = "asd" });

        Assert.True(res.StatusCode == System.Net.HttpStatusCode.BadRequest);

        var content = await res.Content.ReadFromJsonAsync<Microsoft.AspNetCore.Mvc.ProblemDetails>();
        Assert.NotNull(content);
        Assert.Equal("One or more validation errors occurred.", content.Detail);
        Assert.Equal("Validation error", content.Title);
        Assert.Equal(400, content.Status);
        Assert.Equal("/hello", content.Instance);
        var errors = content.Extensions["errors"];
        var errorsString = errors!.ToString()!;
        var errorsArray = JsonConvert.DeserializeObject<ValidationError[]>(errorsString);

        Assert.NotNull(errorsArray);
        Assert.Single(errorsArray);
        Assert.Contains(errorsArray, _ =>
            _.PropertyName == "Input" &&
            _.Message == "The length of 'Input' must be at least 5 characters. You entered 3 characters." &&
            _.ErrorCode == "MinimumLengthValidator"
        );
    }

    [Fact]
    public async Task Update_Hello_World()
    {
        await _testBase.ResetDbAsync();
        var content = await InsertHelloWorld("World");

        var input = new UpdateHelloInput { Input = "World!", RowVersion = content?.RowVersion };
        var res = await _testBase.HttpClient.PatchAsJsonAsync("/api/hello/" + content?.Id, input);
        var updateContent = await res.Content.ReadFromJsonAsync<SqlResult>();
        Assert.Equal(System.Net.HttpStatusCode.OK, res.StatusCode);
        Assert.NotNull(updateContent);
        Assert.NotNull(content);
        Assert.Equal(content.Id, updateContent.Id);
    }

    [Fact]
    public async Task Update_Hello_World_Conflict()
    {
        await _testBase.ResetDbAsync();
        var content = await InsertHelloWorld("World");

        var input = new UpdateHelloInput { Input = "World!", RowVersion = new byte[] { 1, 2, 3 } };
        var res = await _testBase.HttpClient.PatchAsJsonAsync("/api/hello/" + content?.Id, input);
        var updateContent = await res.Content.ReadFromJsonAsync<Microsoft.AspNetCore.Mvc.ProblemDetails>();

        Assert.Equal(System.Net.HttpStatusCode.Conflict, res.StatusCode);
        Assert.NotNull(updateContent);
        Assert.NotNull(content);
        Assert.Equal("Item has already been updated by other user.", updateContent.Detail);
        Assert.Equal("Update conflict", updateContent.Title);
        Assert.Equal(409, updateContent.Status);
        Assert.Equal("/hello/" + content.Id, updateContent.Instance);
    }

    [Fact]
    public async Task Update_Hello_World_Bad_Request()
    {
        await _testBase.ResetDbAsync();
        var content = await InsertHelloWorld("World");

        var input = new UpdateHelloInput();
        var res = await _testBase.HttpClient.PatchAsJsonAsync("/api/hello/" + content?.Id, input);
        var updateContent = await res.Content.ReadFromJsonAsync<Microsoft.AspNetCore.Mvc.ProblemDetails>();

        Assert.NotNull(updateContent);
        Assert.Equal("One or more validation errors occurred.", updateContent.Detail);
        Assert.Equal("Validation error", updateContent.Title);
        Assert.Equal(400, updateContent.Status);
        Assert.Equal("/hello/" + content?.Id, updateContent.Instance);
        var errors = updateContent.Extensions["errors"];
        var errorsString = errors!.ToString()!;
        var errorsArray = JsonConvert.DeserializeObject<ValidationError[]>(errorsString);

        Assert.NotNull(errorsArray);
        Assert.Contains(errorsArray, _ =>
            _.PropertyName == "Input" &&
            _.Message == "'Input' must not be empty." &&
            _.ErrorCode == "NotEmptyValidator"
        );
        Assert.Contains(errorsArray, _ =>
            _.PropertyName == "RowVersion" &&
            _.Message == "'Row Version' must not be empty." &&
            _.ErrorCode == "NotEmptyValidator"
        );
    }

    [Fact]
    public async Task Delete_Hello_World()
    {
        await _testBase.ResetDbAsync();
        var content = await InsertHelloWorld("World");

        var res = await _testBase.HttpClient.DeleteAsync("/api/hello/" + content?.Id);
        Assert.Equal(System.Net.HttpStatusCode.NoContent, res.StatusCode);
    }

    [Fact]
    public async Task Delete_Hello_World_That_Does_Not_Exist()
    {
        await _testBase.ResetDbAsync();
        var id = 111111111;
        var res = await _testBase.HttpClient.DeleteAsync("/api/hello/" + id);
        var deleteContent = await res.Content.ReadFromJsonAsync<Microsoft.AspNetCore.Mvc.ProblemDetails>();
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, res.StatusCode);
        Assert.NotNull(deleteContent);
        Assert.Equal("The item could not be deleted. Its either already deleted or the input ID is incorrect.", deleteContent.Detail);
        Assert.Equal("Unable to delete", deleteContent.Title);
        Assert.Equal(400, deleteContent.Status);
        Assert.Equal("/hello/" + id, deleteContent.Instance);
        Assert.Empty(deleteContent.Extensions);
    }

    [Fact]
    public async Task Delete_Hello_World_Bad_Request()
    {
        await _testBase.ResetDbAsync();
        var res = await _testBase.HttpClient.DeleteAsync("/api/hello/0");
        var deleteContent = await res.Content.ReadFromJsonAsync<Microsoft.AspNetCore.Mvc.ProblemDetails>();
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, res.StatusCode);
        Assert.NotNull(deleteContent);
        Assert.Equal("One or more validation errors occurred.", deleteContent.Detail);
        Assert.Equal("Validation error", deleteContent.Title);
        Assert.Equal(400, deleteContent.Status);
        Assert.Equal("/hello/0", deleteContent.Instance);
        var errors = deleteContent.Extensions["errors"];
        var errorsString = errors!.ToString()!;
        var errorsArray = JsonConvert.DeserializeObject<ValidationError[]>(errorsString);

        Assert.NotNull(errorsArray);
        Assert.Contains(errorsArray, _ =>
            _.PropertyName == "Id" &&
            _.Message == "'Id' must be greater than '0'." &&
            _.ErrorCode == "GreaterThanValidator"
        );
    }

    private async Task<SqlResult?> InsertHelloWorld(string input)
    {
        var res = await _testBase.HttpClient.PostAsJsonAsync("/api/hello", new InsertHelloCommand { Input = input });
        Assert.Equal(System.Net.HttpStatusCode.Created, res.StatusCode);
        return await res.Content.ReadFromJsonAsync<SqlResult>();
    }
}