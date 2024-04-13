var builder = WebApplication.CreateBuilder(args);
var isDevelopment = builder.Environment.IsDevelopment();
var corsOrigin = builder.Configuration["CorsOrigin"];
var connectionString = builder.Configuration.GetConnectionString("WiniDb");

ArgumentNullException.ThrowIfNullOrWhiteSpace(corsOrigin);
ArgumentNullException.ThrowIfNullOrWhiteSpace(connectionString);

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
builder.Services
    .AddAuthorizationBuilder()
    .AddDefaultPolicy("default", _ => _.RequireAuthenticatedUser());

if (builder.Environment.IsEnvironment("IntegrationTests"))
{
    builder.Services.AddScoped<IAuthenticationService, TestAuthenticationService>();
    builder.Services.AddScoped<Domain.Wini.Interfaces.IAuthorizationService, TestAuthorizationService>();
    builder.Services.AddSingleton<IAuthorizerValidationService, TestAuthorizerValidationService>();
    builder.Services.AddSingleton<IBookingPeriodValidationService, TestBookingPeriodValidationService>();
    builder.Services.AddSingleton<IAccountingValidationService, TestAccountingValidationService>();
    builder.Services.AddScoped<IAttachmentService, TestAttachmentService>();
    builder.Services.AddSingleton<IPolicyEvaluator, DisableAuthenticationPolicyEvaluator>();
}
else
{
    builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
    builder.Services.AddScoped<Domain.Wini.Interfaces.IAuthorizationService, TestAuthorizationService>();
    builder.Services.AddSingleton<IAuthorizerValidationService, TestAuthorizerValidationService>();
    builder.Services.AddSingleton<IBookingPeriodValidationService, TestBookingPeriodValidationService>();
    builder.Services.AddSingleton<IAccountingValidationService, TestAccountingValidationService>();
    builder.Services.AddScoped<IAttachmentService, TestAttachmentService>();
}

builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

builder.Services.AddProblemDetails();
builder.Services.AddCors();

builder.Services.AddDatabaseServices(connectionString);
builder.Services.AddAppLogicAndDomainServices();

var app = builder.Build();

if (isDevelopment)
{
    app.UseHttpLogging();
    app.UseDeveloperExceptionPage();
}

app.UseCors(_ => _.AllowAnyHeader().AllowAnyMethod().WithOrigins(corsOrigin).AllowCredentials());
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("api/booking/{id}", BookingEndpoints.GetAsync).RequireAuthorization();
app.MapPatch("api/booking/{id}", BookingEndpoints.PatchAsync).RequireAuthorization();
app.MapDelete("api/booking/{id}", BookingEndpoints.DeleteAsync).RequireAuthorization();
app.MapPost("api/booking", BookingEndpoints.PostAsync).RequireAuthorization();
app.MapPatch("api/booking/{id}/comment", CommentEndpoints.PatchAsync).RequireAuthorization();
app.MapPost("api/booking/{id}/comment", CommentEndpoints.PostAsync).RequireAuthorization();
app.MapDelete("api/booking/{id}/comment", CommentEndpoints.DeleteAsync).RequireAuthorization();
app.MapPatch("api/booking/{id}/recipient", RecipientMessageEndpoints.PatchAsync).RequireAuthorization();
app.MapPost("api/booking/{id}/recipient", RecipientMessageEndpoints.PostAsync).RequireAuthorization();
app.MapDelete("api/booking/{id}/recipient", RecipientMessageEndpoints.DeleteAsync).RequireAuthorization();
app.MapPost("api/booking/{id}/attachment", AttachmentEndpoints.PostAsync).DisableAntiforgery().RequireAuthorization(); // Cant get antiforgery token to work...
app.MapDelete("api/booking/{id}/attachment", AttachmentEndpoints.DeleteAsync).RequireAuthorization();
app.MapPatch("api/booking/{id}/status", BookingStatusEndpoints.PatchAsync).RequireAuthorization();
app.MapGet("api/booking/{id}/validate", ValidationEndpoints.ValidateByIdAsync).RequireAuthorization();
app.MapPost("api/booking/validate", ValidationEndpoints.ValidateNewAsync).RequireAuthorization();

app.MapGet("api/masterdata", MasterDataEndpoints.GetAllCompaniesAsync).RequireAuthorization();

app.Run();
public partial class Program;