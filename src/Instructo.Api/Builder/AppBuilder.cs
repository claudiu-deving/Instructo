namespace Api.Builder;

public static class AppBuilder
{
    public static WebApplication BuildApp(string[] args)
    {
        var isAspireEnabled = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("USING_ASPIRE"));

        var builder = WebApplication.CreateBuilder(args);

        // Only add Aspire service defaults if running with Aspire orchestrator

        if(isAspireEnabled)
            builder.AddServiceDefaults();
        builder.AddLogging();

        builder.AddObservability();

        builder.AddDatabase();

        builder.ConfigureSeeding();

        builder.AddMediator();

        builder.AddValidators();

        builder.AddControllers();

        builder.ConfigureCaching();

        builder.ConfigureRateLimiting();

        builder.ConfigureCors();

        builder.AddAuthentification();

        builder.AddAuthorization();

        builder.AddRepositories();

        return builder.Build();
    }
}
