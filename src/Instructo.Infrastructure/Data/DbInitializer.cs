using System.Reflection;

using Domain.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Data;

public static class DbInitializer
{
    /// <summary>
    /// Applies pending migrations and seeds initial data
    /// </summary>
    public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider, ILogger logger)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        try
        {
            // Apply any pending migrations
            logger.LogInformation("Applying database migrations...");
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully.");

            // Seed roles and users
            await SeedRolesAndAdminUser(serviceProvider);

            // Seed schools if configured
            await SeedSchoolsIfConfiguredAsync(serviceProvider, logger);

            logger.LogInformation("Database seeding completed successfully.");
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }

    /// <summary>
    /// Alternative method for development/testing - ensures database is created
    /// </summary>
    public static async Task EnsureDatabaseCreatedAsync(IServiceProvider serviceProvider, ILogger logger)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        try
        {
            // Apply any pending migrations
            logger.LogInformation("Applying database migrations...");
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully.");

            // Seed roles and users if not already done
            await SeedRolesAndAdminUser(serviceProvider);

            // Check if we should use legacy SQL seeding or new SchoolSeeder
            var seedingOptions = scope.ServiceProvider.GetService<IOptions<SeedingOptions>>()?.Value??new SeedingOptions();

            if(seedingOptions.UseLegacyTestData)
            {
                await SeedLegacyTestDataAsync(context, logger);
            }
            else
            {
                await SeedSchoolsIfConfiguredAsync(serviceProvider, logger);
            }

            logger.LogInformation("Database seeding completed successfully.");
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "An error occurred while ensuring database creation.");
            throw;
        }
    }

    private static async Task SeedSchoolsIfConfiguredAsync(IServiceProvider serviceProvider, ILogger logger)
    {
        using var scope = serviceProvider.CreateScope();
        var seedingOptions = scope.ServiceProvider.GetService<IOptions<SeedingOptions>>()?.Value??new SeedingOptions();

        if(!seedingOptions.EnableSchoolSeeding)
        {
            logger.LogInformation("School seeding is disabled in configuration.");
            return;
        }

        try
        {
            var schoolSeeder = SchoolSeeder.Create(serviceProvider);
            await schoolSeeder.SeedSchoolsAsync(seedingOptions.MinimumSchoolCount, seedingOptions.ForceReseed);
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding schools.");
            // Don't throw in development - log the error but continue
            var environment = scope.ServiceProvider.GetService<IHostEnvironment>();
            if(environment?.IsProduction()==true)
            {
                throw;
            }
        }
    }

    private static async Task SeedLegacyTestDataAsync(AppDbContext context, ILogger logger)
    {
        try
        {
            logger.LogInformation("Using legacy SQL test data seeding...");

            context.Database.BeginTransaction();
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Infrastructure.Data.Hardcoded.InsertTestSchools.sql";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if(stream==null)
            {
                logger.LogWarning("Legacy test data SQL file not found. Skipping legacy seeding.");
                return;
            }

            using var reader = new StreamReader(stream);
            var sqlContent = await reader.ReadToEndAsync();

            if(!string.IsNullOrWhiteSpace(sqlContent))
            {
                await context.Database.ExecuteSqlRawAsync(sqlContent);
                context.Database.CommitTransaction();
                logger.LogInformation("Legacy test data seeded successfully.");
            }
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Failed to seed legacy test data.");
            context.Database.RollbackTransaction();
            throw;
        }
    }

    public static async Task SeedRolesAndAdminUser(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        if(roleManager.Roles.Any())
            return;
        // Create roles
        string[] roleNames = ["Admin", "Owner", "Instructor", "Student"];

        await roleManager.CreateAsync(new ApplicationRole() { Name="IronMan", Description="I am Ironman" });
        await roleManager.CreateAsync(new ApplicationRole() { Name="Admin", Description="Can manage a school, cannot delete it" });
        await roleManager.CreateAsync(new ApplicationRole() { Name="Owner", Description="Can manage a school, can delete it" });
        await roleManager.CreateAsync(new ApplicationRole() { Name="Instructor", Description="Can manage session" });
        await roleManager.CreateAsync(new ApplicationRole() { Name="Student", Description="Can view session" });

        var ironMan = new ApplicationUser()
        {
            Id=Guid.Parse("8fb090d2-ad97-41d0-86ce-08ddc4a5a731"),
            Email="claudiu.c.strugar@gmail.com",
            LastName="Strugar",
            FirstName="Claudiu",
            PhoneNumber="1234567890",
            UserName="claudiu.c.strugar@gmail.com"
        };
        await userManager.CreateAsync(ironMan, "Password123!");
        await userManager.AddToRoleAsync(ironMan, "IronMan");
    }
}

