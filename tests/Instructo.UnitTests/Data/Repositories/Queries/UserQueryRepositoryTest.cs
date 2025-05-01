using FluentAssertions;
using Infrastructure.Data.Repositories.Queries;
using JetBrains.Annotations;

namespace Instructo.UnitTests.Data.Repositories.Queries;

[TestSubject(typeof(UserQueryRepository))]
public class UserQueryRepositoryTest
{
    private readonly UserQueryRepository _sut;

    public UserQueryRepositoryTest()
    {
        const string connectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=Instructo_Test;Trusted_Connection=True;MultipleActiveResultSets=true";
        _sut = new UserQueryRepository(connectionString);
    }
    
    [Fact]
    public async Task SimpleTest()
    {
        var x = (await _sut.GetUsers()).ToList();
        x.Count.Should().Be(77);
        x[0].FirstName = "Claudiu";
        x[0].Role.Should().NotBeNull();
        x[0].Role!.Name.Should().Be("IronMan");
    }
}