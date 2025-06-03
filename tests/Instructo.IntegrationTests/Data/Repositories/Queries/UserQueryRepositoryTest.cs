using Domain.Entities;
using Domain.Entities.SchoolEntities;
using Domain.ValueObjects;
using FluentAssertions;
using Infrastructure.Data;
using Infrastructure.Data.Repositories.Queries;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Testcontainers.MsSql;

namespace Instructo.UnitTests.Data.Repositories.Queries;

[TestSubject(typeof(UserQueryRepository))]
public class UserQueryRepositoryTest
{
    private readonly AppDbContext _context;
    private readonly UserQueryRepository _sut;

    public UserQueryRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(
                "Server=(localdb)\\MSSQLLocalDB;Database=Instructo_Test2;Trusted_Connection=True;MultipleActiveResultSets=true")
            .Options;
        _context = new AppDbContext(options);
        _sut = new UserQueryRepository(_context);
    }

    [Fact]
    public async Task SimpleTest()
    {
        var users = (await _sut.GetUsers()).ToList();
        users.Count.Should().Be(78);
        users[0].FirstName = "Claudiu";
        users[0].Role.Should().NotBeNull();
        users[0].Role!.Name.Should().Be("IronMan");
    }
}

public class IntegrationTestFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer;
    private string _connectionString = string.Empty;

    public IntegrationTestFixture()
    {
        _dbContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("YourStrong@Passw0rd")
            .WithCleanUp(true)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        _connectionString = _dbContainer.GetConnectionString();

        // Create a scope to set up the database
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Ensure database is created and migrations are applied
        await context.Database.EnsureCreatedAsync();

        // Apply any pending migrations
        if ((await context.Database.GetPendingMigrationsAsync()).Any()) await context.Database.MigrateAsync();

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
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null)
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
        if (!await context.Schools.AnyAsync())
        {
            var testSchools = new[]
            {
                new School(new ApplicationUser(),
                    SchoolName.Wrap("Test Driving School 1"),
                    LegalName.Wrap("Name"),
                    Email.Create("<EMAIL>").Value!,
                    PhoneNumber.Create("555-123-4567").Value!,
                    [], BussinessHours.Empty,
                    [], [], null),
                new School(new ApplicationUser(),
                    SchoolName.Wrap("Test Driving School 2"),
                    LegalName.Wrap("Name2"),
                    Email.Create("<EMAIL>").Value!,
                    PhoneNumber.Create("555-123-4567").Value!,
                    [], BussinessHours.Empty,
                    [], [], null)
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

// Alternative: In-Memory Database Fixture (faster but less realistic)
public class InMemoryTestFixture : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Add in-memory database
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
                options.EnableSensitiveDataLogging();
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

    public async Task<AppDbContext> GetDbContextAsync()
    {
        var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Ensure database is created for in-memory
        await context.Database.EnsureCreatedAsync();

        return context;
    }
}

// Base class for integration tests
public abstract class IntegrationTestBase : IClassFixture<IntegrationTestFixture>
{
    protected readonly HttpClient Client;
    protected readonly IntegrationTestFixture Fixture;

    protected IntegrationTestBase(IntegrationTestFixture fixture)
    {
        Fixture = fixture;
        Client = fixture.CreateClient();
    }

    protected async Task<T> ExecuteDbContextAsync<T>(Func<AppDbContext, Task<T>> action)
    {
        using var context = Fixture.GetDbContext();
        return await action(context);
    }

    protected async Task ExecuteDbContextAsync(Func<AppDbContext, Task> action)
    {
        using var context = Fixture.GetDbContext();
        await action(context);
    }
}

// Test Collection Definition (for xUnit)
[CollectionDefinition("Integration Tests")]
public class IntegrationTestCollection : ICollectionFixture<IntegrationTestFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}