using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data;

/// <summary>
/// Background service that handles database migrations on application startup
/// </summary>
public class DatabaseMigrationService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseMigrationService> _logger;

    public DatabaseMigrationService(IServiceProvider serviceProvider, ILogger<DatabaseMigrationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        try
        {
            _logger.LogInformation("Starting database migration...");
            
            // Check if database exists
            var canConnect = await context.Database.CanConnectAsync(stoppingToken);
            if (!canConnect)
            {
                _logger.LogWarning("Cannot connect to database. Database might not exist.");
                return;
            }

            // Get pending migrations
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync(stoppingToken);
            
            if (pendingMigrations.Any())
            {
                _logger.LogInformation("Found {Count} pending migrations: {Migrations}", 
                    pendingMigrations.Count(), 
                    string.Join(", ", pendingMigrations));
                
                // Apply migrations
                await context.Database.MigrateAsync(stoppingToken);
                _logger.LogInformation("Database migration completed successfully.");
            }
            else
            {
                _logger.LogInformation("No pending migrations found.");
            }

            // Seed initial data
            await DbInitializer.SeedRolesAndAdminUser(_serviceProvider);
            _logger.LogInformation("Database seeding completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during database migration.");
            throw;
        }
    }
}