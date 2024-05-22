namespace AppLogic.WiniLogic.Startup;

public static class Startup
{
    public static void AddAppLogicAndDomainServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(InsertNewBookingCommand).Assembly);
            cfg.AddOpenBehavior(typeof(AuthenticationBehaviour<,>));
        });

        services.AddScoped<IDateTimeService, DateTimeService>();

        services.AddScoped<IBookingValidationService, BookingValidationService>();
    }
}