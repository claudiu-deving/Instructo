using System.Reflection;

using Bogus;

using Domain.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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

            // Seed roles and users
            await SeedRolesAndAdminUser(serviceProvider);
            context.Database.BeginTransaction();
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Infrastructure.Data.Hardcoded.InsertTestSchools.sql";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            var sqlContent = reader.ReadToEnd();
            context.Database.ExecuteSqlRaw(sqlContent);
            context.Database.CommitTransaction();
            logger.LogInformation("Database seeding completed successfully.");
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "An error occurred while ensuring database creation.");
            throw;
        }
    }

    public static async Task SeedRolesAndAdminUser(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Create roles
        string[] roleNames = ["Admin", "Owner", "Instructor", "Student"];

        await roleManager.CreateAsync(new ApplicationRole() { Name="IronMan", Description="I am Ironman" });
        await roleManager.CreateAsync(new ApplicationRole() { Name="Admin", Description="Can manage a school, cannot delete it" });
        await roleManager.CreateAsync(new ApplicationRole() { Name="Owner", Description="Can manage a school, can delete it" });
        await roleManager.CreateAsync(new ApplicationRole() { Name="Instructor", Description="Can manage session" });
        await roleManager.CreateAsync(new ApplicationRole() { Name="Student", Description="Can view session" });

        var ironMan = new ApplicationUser()
        {
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

