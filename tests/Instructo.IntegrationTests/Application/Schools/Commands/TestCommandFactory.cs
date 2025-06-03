using Domain.Dtos.Link;
using Domain.Dtos.School;

using Application.Schools.Commands.CreateSchool;

namespace Instructo.UnitTests.Application.Schools.Commands
{
    /// <summary>
    /// Factory for creating test command instances
    /// </summary>
    public static class TestCommandFactory
    {
        public static CreateSchoolCommand CreateValidSchoolCommand(
            string name = "Test Driving School",
            string email = "test@example.com")
        {
            var websiteLink = new WebsiteLinkReadDto
            {
                Url="https://example.com",
                Name="Website",
                Description="Official website",
                IconData=new Domain.Dtos.Image.ImageReadDto
                {
                    FileName="website-icon.png",
                    Url="/path/to/valid/website-icon.png",
                    ContentType="image/png"
                }
            };
            List<SocialMediaLinkDto> socialMediaLinks = [new SocialMediaLinkDto
            {
                Url="https://facebook.com/testschool",
                SocialPlatformName="Fcebook"
            }];
            var request = CreateSchoolCommand.Create(new CreateSchoolCommandDto(
                name,
                $"{name} LLC",
                    email,
                "Test@test",
                 "Password123!",
                 "Name",
                 "LastName",
                "Test City",
                "123 Test St, Test City, TC 12345",
                "123-456-7890",
                "image/png",
                "/path/to/valid/image.png",
                [],
                websiteLink,
                socialMediaLinks,
                [], ["A1"], []
            ));
            if(request.IsError)
            {
                throw new InvalidOperationException($"Failed to create valid command: {string.Join(", ", request.Errors.Select(e => e.Message))}");
            }
            return request.Value!;
        }
    }
}
