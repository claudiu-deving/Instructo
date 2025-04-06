using Instructo.Application.Schools.Commands.CreateSchool;
using Instructo.Domain.Dtos.Link;
using Instructo.Domain.Dtos.School;

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
                SocialPlatformName="Facebook"
            }];
            var request = CreateSchoolCommand.Create(new CreateSchoolCommandDto()
            {
                Name=name,
                LegalName=$"{name} LLC",
                OwnerFirstName="Test",
                OwnerLastName="Owner",
                OwnerEmail=email,
                City="Test City",
                Address="123 Test St, Test City, TC 12345",
                OwnerPassword="Password123!",
                PhoneNumber="123-456-7890",
                ImageContentType="image/png",
                ImagePath="/path/to/valid/image.png",
                WebsiteLink=websiteLink,
                SocialMediaLinks=socialMediaLinks
            });
            if(request.IsError)
            {
                throw new InvalidOperationException($"Failed to create valid command: {string.Join(", ", request.Errors.Select(e => e.Message))}");
            }
            return request.Value!;
        }
    }
}
