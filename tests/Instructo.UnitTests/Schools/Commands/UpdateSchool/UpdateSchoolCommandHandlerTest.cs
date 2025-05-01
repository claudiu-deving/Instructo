using Application.Schools.Commands.UpdateSchool;
using Infrastructure.Data;
using Infrastructure.Data.Repositories.Commands;
using JetBrains.Annotations;

namespace Instructo.UnitTests.Schools.Commands.UpdateSchool;

[TestSubject(typeof(UpdateSchoolCommandHandler))]
public class UpdateSchoolCommandHandlerTest
{

    [Fact]
    public void ValidCommandAsInput_ReturnsModifiedSchool()
    {
        var dbContext = new AppDbContext()
        _sut = new UpdateSchoolCommandHandler(new SchoolCommandRepository());
    }
}