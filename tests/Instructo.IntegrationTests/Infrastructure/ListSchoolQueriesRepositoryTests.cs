using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Infrastructure.Data.Repositories.Queries;

using Instructo.IntegrationTests.Data.Repositories.Queries;

using Xunit.Abstractions;

namespace Instructo.IntegrationTests.Infrastructure;
[Collection("Integration Tests")]
public class ListSchoolQueriesRepositoryTests(
    IntegrationTestFixture integrationTestFixture,
    ITestOutputHelper testOutputHelper) : IntegrationTestBase(integrationTestFixture)
{
    private readonly SchoolQueriesRepository _sut = new SchoolQueriesRepository(
        integrationTestFixture.GetDbContext(),
        new TestLogger(testOutputHelper));

    [Fact]
    public async Task WhenConnectionIsTried_ItConnects()
    {
        var result = await _sut.GetAllAsync();
        Assert.NotNull(result);
        Assert.False(result.IsError);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count());
    }

}
