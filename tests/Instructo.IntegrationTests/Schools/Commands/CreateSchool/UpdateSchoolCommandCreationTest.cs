using Application.Schools.Commands.CreateSchool;
using Application.Schools.Commands.UpdateSchool;

using Domain.Dtos.Link;
using Domain.Dtos.School;
using Domain.ValueObjects;

using FluentAssertions;

using JetBrains.Annotations;

namespace Instructo.IntegrationTests.Schools.Commands.CreateSchool;

[TestSubject(typeof(CreateSchoolCommand))]
public class CreateSchoolCommandCreationTest
{
    [Fact]
    public void CreateCreateSchoolCommandFromValidDto_ReturnsUpdateCommand()
    {
        var validUpdateSchoolDto = new CreateSchoolCommandDto(
            "Test",
            "Test Company SRL",
            "Owner@email.com",
            "contact@schoo.ro",
            "somePass123!",
            "John",
            "Doe",
            "London",
            "123 Street",
            "0758455151",
            "src/image",
            "png.image",
            [],
            WebsiteLinkReadDto.Empty,
            [],
            [],
            ["B"],
            []);

        var createSchoolCommand = CreateSchoolCommand.Create(validUpdateSchoolDto);
        if(createSchoolCommand.IsError)
            createSchoolCommand.IsError.Should()
                .BeFalse($"{string.Join(Environment.NewLine, createSchoolCommand.Errors.ToList())}");
        createSchoolCommand.Value.Should().NotBeNull();
    }
}