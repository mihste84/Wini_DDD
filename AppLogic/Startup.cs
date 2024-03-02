namespace AppLogic.Startup;

public static class Startup
{
    public static void AddAppLogicAndDomainServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(InsertNewBookingCommand).Assembly);
            cfg.AddOpenBehavior(typeof(AuthenticationBehaviour<,>));
            //cfg.AddOpenBehavior(typeof(RequestValidationBehaviour<,>));
        });

        services.AddScoped<IDateTimeService, DateTimeService>();
        //services.AddValidatorsFromAssemblyContaining<InsertHelloCommand>();

        services.AddScoped<IBookingValidationService, BookingValidationService>();
    }
}