using Infrastructure.Data;

using Instructo.IntegrationTests.Infrastructure;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Instructo.IntegrationTests;

public abstract class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    protected readonly HttpClient _client;
    protected readonly SchoolSeeder? _seeder;
    protected readonly AuthenticationHelper? _authentificationHelper;
    private readonly CustomWebApplicationFactory _factory;

    protected IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        _factory=factory;
        _client=factory.CreateClient();
        _seeder=factory.Seeder;
        _authentificationHelper=factory.AuthenticationHelper;
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();

    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
