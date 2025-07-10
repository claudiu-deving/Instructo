using Application.Schools.Commands.CreateSchool;
using Application.Schools.Commands.UpdateSchool;

using Domain.Dtos.Link;
using Domain.Dtos.School;
using Domain.ValueObjects;


namespace Instructo.IntegrationTests.Schools.Commands.CreateSchool;

public class CreateSchoolCommandCreationTest
{
    [Fact]
    public void CreateCreateSchoolCommandFromValidDto_ReturnsUpdateCommand()
    {
        var validUpdateSchoolDto = new CreateSchoolCommandDto(
            Name: "Test",
            LegalName: "Test Company SRL",
            OwnerEmail: "Owner@email.com",
            SchoolEmail: "contact@schoo.ro",
            City: "London",
            Address: "123 Street",
            PhoneNumber: "0758455151",
            ImagePath: "src/image",
            ImageContentType: "png.image",
            Slogan: "Test Slogan",
            Description: "Test Description",
            X: "25.251",
            Y: "42.81",
            PhoneNumberGroups: [],
            WebsiteLink: WebsiteLinkReadDto.Empty,
            SocialMediaLinks: [],
            BussinessHours: [],
            VechiclesCategories: ["B"],
            ArrCertifications: []);

        var createSchoolCommand = CreateSchoolCommand.Create(validUpdateSchoolDto);
        Assert.False(createSchoolCommand.IsError, $"{string.Join(Environment.NewLine, createSchoolCommand.Errors.ToList())}");
        Assert.NotNull(createSchoolCommand.Value);
    }
}