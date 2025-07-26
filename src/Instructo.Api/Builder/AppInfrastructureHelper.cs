using Infrastructure.Data;

namespace Api.Builder;

public static class AppInfrastructureHelper
{
    public static async Task InitializeDatabaseAsync(this WebApplication app, EnvironmentType environmentType)
    {
        // Initialize database with proper migration handling
        using(var scope = app.Services.CreateScope())
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                if(environmentType==EnvironmentType.Production)
                    // In production, apply migrations
                    await DbInitializer.InitializeDatabaseAsync(scope.ServiceProvider, logger);
                else if(environmentType==EnvironmentType.Development || environmentType == EnvironmentType.Staging || environmentType == EnvironmentType.Testing)
                {
                    // In development or staging, ensure database is created
                    await DbInitializer.EnsureDatabaseCreatedAsync(scope.ServiceProvider, logger);
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing the database.");
                if(!app.Environment
                        .IsDevelopment())
                    throw;
            }
        }

    }

    public static void AddDatabase(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("db")??
                        builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    5,
                    TimeSpan.FromSeconds(30),
                    null);
            });
            if(builder.Environment.IsDevelopment()||builder.Environment.IsStaging()||builder.Environment.IsEnvironment("Testing"))
                options.EnableSensitiveDataLogging();
            else
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        builder.Services.AddMiniProfiler(options =>
        {
            options.RouteBasePath="/profiler";
        }).AddEntityFramework();
    }
}
