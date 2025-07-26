using Infrastructure.Data;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Respawn;
using Respawn.Graph;

namespace Instructo.IntegrationTests.Infrastructure;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public AuthenticationHelper? AuthenticationHelper { get; private set; }

    private Respawner _respawner = default!;
    public SchoolSeeder? Seeder { get; private set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
    }

    public async Task InitializeAsync()
    {
        Seeder=SchoolSeeder.Create(Services);
        var connectionString = Services.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection");
        AuthenticationHelper=new AuthenticationHelper(Services);
        _respawner=await Respawner.CreateAsync(connectionString!, new RespawnerOptions()
        {
            DbAdapter=DbAdapter.SqlServer,
            SchemasToInclude= ["dbo"],
            TablesToIgnore=
            [
                "__EFMigrationsHistory",
                "Roles",
                "Users",
                "UserRoles",
                "UserClaims",
                "UserLogins",
                "UserTokens",
                "RoleClaims",
                "VehicleCategories",
                "ARRCertificates",
                "Counties",
                "Cities",
                "Transmissions" ,

            ]
        });
    }

    public async Task ResetDatabaseAsync()
    {
        var connectionString = Services.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection");
        await _respawner.ResetAsync(connectionString!);
    }

    public new Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
