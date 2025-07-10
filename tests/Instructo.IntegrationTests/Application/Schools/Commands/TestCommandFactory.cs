using Application.Schools.Commands.CreateSchool;
using Domain.Dtos.Image;
using Domain.Dtos.Link;
using Domain.Dtos.School;

namespace Instructo.UnitTests.Application.Schools.Commands;

/// <summary>
///     Factory for creating test command instances
/// </summary>
public static class TestCommandFactory
{
    public static CreateSchoolCommand CreateValidSchoolCommand(
        string name = "Test Driving School",
        string email = "test@example.com")
    {
        var websiteLink = new WebsiteLinkReadDto
        {
            Url = "https://example.com",
            Name = "Website",
            Description = "Official website",
            IconData = new ImageReadDto
            {
                FileName = "website-icon.png",
                Url = "/path/to/valid/website-icon.png",
                ContentType = "image/png"
            }
        };
        List<SocialMediaLinkDto> socialMediaLinks =
        [
            new()
            {
                Url = "https://facebook.com/testschool",
                SocialPlatformName = "Fcebook"
            }
        ];
        var request = CreateSchoolCommand.Create(new CreateSchoolCommandDto(
            Name: name,
            LegalName: $"{name} LLC",
            OwnerEmail: email,
            SchoolEmail: "school@example.com",
            City: "Test City",
            Address: "123 Test St",
            PhoneNumber: "123-456-7890",
            ImagePath: "/path/to/valid/image.png",
            ImageContentType: "image/png",
            Slogan: "Test Slogan",
            Description: "Test Description",
            X: "25.251",
            Y: "42.81",
            PhoneNumberGroups: [],
            WebsiteLink: websiteLink,
            SocialMediaLinks: socialMediaLinks,
            BussinessHours: [],
            VechiclesCategories: ["A1"],
            ArrCertifications: []
        ));
        if (request.IsError)
            throw new InvalidOperationException(
                $"Failed to create valid command: {string.Join(", ", request.Errors.Select(e => e.Message))}");
        return request.Value!;
    }
}