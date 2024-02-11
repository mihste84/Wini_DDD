var builder = WebApplication.CreateBuilder(args);
var isDevelopment = builder.Environment.IsDevelopment();

if (isDevelopment)
{
    builder.Services.AddHttpLogging(_ =>
    {
        _.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
        _.CombineLogs = true;
    });
}

if (builder.Environment.IsEnvironment("IntegrationTests"))
{
    builder.Services.AddScoped<IAuthenticationService, TestAuthenticationService>();
    builder.Services.AddSingleton<IAuthorizationHandler, AllowAnonymous>();
}

builder.Services.Configure<CookieAuthenticationOptions>(o => o.LoginPath = PathString.Empty);

builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

builder.Services.AddProblemDetails();

var connectionString = builder.Configuration.GetConnectionString("WiniDb");
builder.Services.AddDatabaseServices(connectionString);
builder.Services.AddAppLogicServices();

var app = builder.Build();

if (isDevelopment)
{
    app.UseDeveloperExceptionPage();
    app.UseHttpLogging();
}

app.UseRouting();

// app.UseCors();
// app.UseAuthentication();
// app.UseAuthorization();

app.UsePathBase(new PathString("/api"));
app.MapPost("/hello", HelloEndpoints.Post);
app.MapPatch("/hello/{id}", HelloEndpoints.Patch);
app.MapDelete("/hello/{id}", HelloEndpoints.Delete);

app.MapGet("/companies", CompanyEndpoints.GetAllCompanies);

app.Run();

public partial class Program { }