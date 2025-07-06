using Infrastructure.Data;

namespace Instructo.IntegrationTests.Data.Repositories.Queries;

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