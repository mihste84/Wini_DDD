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
    public async Task Hello_World()
    {
        // await _testBase.ResetDbAsync();
        var res = await _testBase.HttpClient.GetAsync("/api/hello/World");
        res.EnsureSuccessStatusCode();
        var content = await res.Content.ReadFromJsonAsync<BaseResponse<string>>();
        Assert.NotNull(content);
        Assert.Equal("Hello World", content.Value);
    }

    [Fact]
    public async Task Hello_World_Bad_Request()
    {
        // await _testBase.ResetDbAsync();
        var res = await _testBase.HttpClient.GetAsync("/api/hello/asd");

        Assert.True(res.StatusCode == System.Net.HttpStatusCode.BadRequest);

        var content = await res.Content.ReadFromJsonAsync<Microsoft.AspNetCore.Mvc.ProblemDetails>();
        Assert.NotNull(content);
        Assert.Equal("One or more validation errors occurred.", content.Detail);
        Assert.Equal("Validation error", content.Title);
        Assert.Equal(400, content.Status);
        Assert.Equal("/hello/asd", content.Instance);
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
}