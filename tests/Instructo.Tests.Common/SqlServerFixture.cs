using Testcontainers.MsSql;

using Xunit;

namespace Instructo.Tests.Common;

public class SqlServerTestFixture : IAsyncLifetime
{
    public MsSqlContainer SqlContainer { get; }

    public SqlServerTestFixture()
    {
        SqlContainer=new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("StrongP@ssw0rd!")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await SqlContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await SqlContainer.StopAsync();
    }
}
