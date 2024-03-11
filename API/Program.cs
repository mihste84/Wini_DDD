using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
builder.Services.AddAuthorization();

if (builder.Environment.IsEnvironment("IntegrationTests"))
{
    builder.Services.AddScoped<IAuthenticationService, TestAuthenticationService>();
    builder.Services.AddScoped<Domain.Wini.Interfaces.IAuthorizationService, TestAuthorizationService>();
    builder.Services.AddSingleton<IAuthorizerValidationService, TestAuthorizerValidationService>();
    builder.Services.AddSingleton<IBookingPeriodValidationService, TestBookingPeriodValidationService>();
    builder.Services.AddSingleton<IAccountingValidationService, TestAccountingValidationService>();
    builder.Services.AddScoped<IAttachmentService, TestAttachmentService>();
}
else
{
    builder.Services
        .AddAuthorizationBuilder()
        .AddDefaultPolicy("default", _ => _.RequireAuthenticatedUser());
}

builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

builder.Services.AddProblemDetails();

var connectionString = builder.Configuration.GetConnectionString("WiniDb");
builder.Services.AddDatabaseServices(connectionString);
builder.Services.AddAppLogicAndDomainServices();

var app = builder.Build();

if (isDevelopment)
{
    app.UseHttpLogging();
}

app.UsePathBase(new PathString("/api"));
app.MapGet("/booking/{id}", BookingEndpoints.GetAsync);
app.MapPatch("/booking/{id}", BookingEndpoints.PatchAsync);
app.MapDelete("/booking/{id}", BookingEndpoints.DeleteAsync);
app.MapPost("/booking", BookingEndpoints.PostAsync);
app.MapPatch("/booking/{id}/comment", CommentEndpoints.PatchAsync);
app.MapPost("/booking/{id}/comment", CommentEndpoints.PostAsync);
app.MapDelete("/booking/{id}/comment", CommentEndpoints.DeleteAsync);
app.MapPatch("/booking/{id}/recipient", RecipientMessageEndpoints.PatchAsync);
app.MapPost("/booking/{id}/recipient", RecipientMessageEndpoints.PostAsync);
app.MapDelete("/booking/{id}/recipient", RecipientMessageEndpoints.DeleteAsync);
app.MapPost("/booking/{id}/attachment", AttachmentEndpoints.PostAsync).DisableAntiforgery(); // Cant get antiforgery token to work...
app.MapDelete("/booking/{id}/attachment", AttachmentEndpoints.DeleteAsync);
app.MapPatch("/booking/{id}/status/", BookingStatusEndpoints.PatchAsync);

app.MapGet("/companies", CompanyEndpoints.GetAllCompaniesAsync);

app.Run();
public partial class Program;