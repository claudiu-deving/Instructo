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

        // Apply migrations (this will seed VehicleCategories and other data from migrations)
        await context.Database.MigrateAsync();

        // Seed test data including our custom school data
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

            // ConfigureArrCertificates logging for tests
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Warning);
            });
        });

        builder.UseEnvironment("Staging");
    }

    private static async Task SeedTestDataAsync(IServiceProvider serviceProvider)
    {
        // Seed roles and initial user data
        await DbInitializer.SeedRolesAndAdminUser(serviceProvider);

        // Execute the test school data SQL
        await ExecuteTestSchoolDataAsync(serviceProvider);
    }

    private static async Task ExecuteTestSchoolDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Read the SQL file content
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        var resourceName = "Instructo.IntegrationTests.TestData.InsertTestSchools.sql";

        // Try alternative path if the embedded resource doesn't exist
        string sqlContent;
        using(var stream = assembly.GetManifestResourceStream(resourceName))
        {
            if(stream==null)
            {
                // Fallback: read from file system
                var sqlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..",
                    "..", "src", "Instructo.Infrastructure", "Data", "Hardcoded", "InsertTestSchools.sql");

                if(File.Exists(sqlFilePath))
                {
                    sqlContent=await File.ReadAllTextAsync(sqlFilePath);
                }
                else
                {
                    // Skip seeding if file not found - tests will handle empty state
                    return;
                }
            }
            else
            {
                using var reader = new StreamReader(stream);
                sqlContent=await reader.ReadToEndAsync();
            }
        }

        // Execute the SQL commands
        if(!string.IsNullOrWhiteSpace(sqlContent))
        {
            await context.Database.ExecuteSqlRawAsync(sqlContent);
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

        // Don't remove users, roles, or other reference data as they're expensive to recreate
        await context.SaveChangesAsync();
    }
}