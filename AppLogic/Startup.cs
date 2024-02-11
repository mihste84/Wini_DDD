namespace AppLogic.Startup;

public static class Startup
{
    public static void AddAppLogicServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(InsertHelloCommand).Assembly);
            cfg.AddOpenBehavior(typeof(AuthenticationBehaviour<,>));
            //cfg.AddOpenBehavior(typeof(RequestValidationBehaviour<,>));
        });
        //services.AddValidatorsFromAssemblyContaining<InsertHelloCommand>();
    }
}