using Domain.Entities;
using Domain.Entities.SchoolEntities;
using Domain.ValueObjects;

using Infrastructure.Data;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Testcontainers.MsSql;

namespace Instructo.IntegrationTests.Data.Repositories.Queries;

public class IntegrationTestFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer;
    private string _connectionString = string.Empty;

    public IntegrationTestFixture()
    {
        _dbContainer=new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("YourStrong@Passw0rd")
            .WithCleanUp(true)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        _connectionString=_dbContainer.GetConnectionString();

        // Create a scope to set up the database
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Ensure database is created and migrations are applied
        await context.Database.EnsureCreatedAsync();

        // Seed test data
        await SeedTestDataAsync(scope.ServiceProvider);
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(d => d.ServiceType==typeof(DbContextOptions<AppDbContext>));
            if(descriptor!=null)
                services.Remove(descriptor);

            // Add the test database context
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(_connectionString);
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            });

            // Configure logging for tests
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Warning);
            });
        });

        builder.UseEnvironment("Testing");
    }

    private async Task SeedTestDataAsync(IServiceProvider serviceProvider)
    {
        // Seed roles and initial user data
        await DbInitializer.SeedRolesAndAdminUser(serviceProvider);

        // Add any additional test-specific seeding here
        await SeedTestSchools(serviceProvider);
    }

    private async Task SeedTestSchools(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Add some test schools if they don't exist
        if(!await context.Schools.AnyAsync())
        {
            var testSchools = new[]
            {
                new School(new ApplicationUser(),
                    SchoolName.Wrap("Test Driving School 1"),
                    LegalName.Wrap("Name"),
                    Email.Create("<EMAIL>").Value!,
                    PhoneNumber.Create("555-123-4567").Value!,
                    [], BussinessHours.Empty,
                    [], [], null,new City(){Name = "Cluj-Napoca"},new Slogan(""),new Description(""),new Address("test",new NetTopologySuite.Geometries.Point(new NetTopologySuite.Geometries.Coordinate(0,0)))),
                new School(new ApplicationUser(),
                    SchoolName.Wrap("Test Driving School 2"),
                    LegalName.Wrap("Name2"),
                    Email.Create("<EMAIL>").Value!,
                    PhoneNumber.Create("555-123-4567").Value!,
                    [], BussinessHours.Empty,
                    [], [], null,new City(){Name = "Cluj-Napoca"},new Slogan(""),new Description(""),new Address("test",new NetTopologySuite.Geometries.Point(new NetTopologySuite.Geometries.Coordinate(0,0))))
            };

            context.Schools.AddRange(testSchools);
            await context.SaveChangesAsync();
        }
    }

    // Helper method to get a fresh database context for tests
    public AppDbContext GetDbContext()
    {
        var scope = Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    // Helper method to clean up data between tests if needed
    public async Task ResetDatabaseAsync()
    {
        using var context = GetDbContext();

        // Remove test-specific data but keep seeded reference data
        context.Schools.RemoveRange(context.Schools);

        // Don't remove users, roles, or other reference data as they're expensive to recreate
        await context.SaveChangesAsync();
    }
}