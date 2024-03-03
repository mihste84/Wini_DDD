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
    builder.Services.AddScoped<Domain.Wini.Interfaces.IAuthorizationService, TestAuthorizationService>();
    builder.Services.AddSingleton<IAuthorizationHandler, AllowAnonymous>();
    builder.Services.AddSingleton<IAuthorizerValidationService, TestAuthorizerValidationService>();
    builder.Services.AddSingleton<IBookingPeriodValidationService, TestBookingPeriodValidationService>();
    builder.Services.AddSingleton<IAccountingValidationService, TestAccountingValidationService>();
}
builder.Services.Configure<CookieAuthenticationOptions>(o => o.LoginPath = PathString.Empty);

builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

builder.Services.AddProblemDetails();

var connectionString = builder.Configuration.GetConnectionString("WiniDb");
builder.Services.AddDatabaseServices(connectionString);
builder.Services.AddAppLogicAndDomainServices();

var app = builder.Build();

if (isDevelopment)
{
    app.UseDeveloperExceptionPage();
    app.UseHttpLogging();
}

app.UseRouting();

// app.UseAuthentication();
// app.UseAuthorization();

app.UsePathBase(new PathString("/api"));

app.MapGet("/booking/{id}", BookingEndpoints.GetAsync);
app.MapPatch("/booking/{id}", BookingEndpoints.PatchAsync);
app.MapDelete("/booking/{id}", BookingEndpoints.DeleteAsync);
app.MapPost("/booking", BookingEndpoints.PostAsync);
app.MapPatch("/booking/{id}/comment", CommentEndpoints.PatchAsync);
app.MapPatch("/booking/{id}/recipient", RecipientMessageEndpoints.PatchAsync);

app.MapPatch("/bookingstatus/{id}", BookingStatusEndpoints.PatchAsync);

app.MapGet("/companies", CompanyEndpoints.GetAllCompaniesAsync);

app.Run();

public partial class Program;