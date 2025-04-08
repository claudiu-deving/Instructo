using Domain.Dtos.Link;

using FluentAssertions;

using Instructo.Application.Schools.Commands.CreateSchool;

namespace Instructo.UnitTests.Application.Schools.Commands.CreateSchools;

public class CreateSchoolCommandCreationTests
{

    [Fact]
    public void ValidInputShouldCreateANewCommand()
    {
        var request = CreateSchoolCommand.Create(new CreateSchoolCommandDto()
        {
            Name="Test Driving School",
            LegalName="Test Driving School LLC",
            OwnerFirstName="Test",
            OwnerLastName="Owner",
            OwnerEmail="Owner@EMail.com",
            SchoolEmail="Scgool@Email.com",
            OwnerPassword="123xad21!~",
            City="Test City",
            Address="123 Test St",
            PhoneNumber="123-456-7890",
            ImagePath="/path/to/valid/image.png",
            ImageContentType="image/png",
            PhoneNumberGroups= [],
            WebsiteLink=new WebsiteLinkReadDto
            {
                Url="https://example.com",
                Name="Website",
                Description="Official website",
                IconData=new Domain.Dtos.Image.ImageReadDto
                {
                    Url="/path/to/valid/website-icon.png",
                    ContentType="image/png"
                }
            },
            SocialMediaLinks=new List<SocialMediaLinkDto>
            {
                new SocialMediaLinkDto
                {
                    Url = "https://facebook.com/test-school",
                    SocialPlatformName = "Facebook"
                }
            }
        });
        if(request.IsError)
        {
            throw new InvalidOperationException($"Failed to create valid command: {string.Join(", ", request.Errors.Select(e => e.Message))}");
        }
        var command = request.Value!;
        Assert.NotNull(command);
        Assert.Equal("Test Driving School", command.Name);
        Assert.Equal("Test Driving School LLC", command.LegalName);
        Assert.Equal("Test", command.OwnerFirstName);

    }

    [Fact]
    public void LegalNameStartsWithNonCapitalLetter_InvalidatesInput()
    {
        var request = CreateSchoolCommand.Create(new CreateSchoolCommandDto()
        {
            Name="Test Driving School",
            LegalName="test Driving School LLC",
            OwnerFirstName="Test",
            OwnerLastName="Owner",
            OwnerEmail="Owner@EMail.com",
            SchoolEmail="Scgool@Email.com",
            OwnerPassword="123xad21!~",
            City="Test City",
            PhoneNumberGroups= [],
            Address="123 Test St",
            PhoneNumber="123-456-7890",
            ImagePath="/path/to/valid/image.png",
            ImageContentType="image/png",
            WebsiteLink=new WebsiteLinkReadDto
            {
                Url="https://example.com",
                Name="Website",
                Description="Official website",
                IconData=new Domain.Dtos.Image.ImageReadDto
                {
                    Url="/path/to/valid/website-icon.png",
                    ContentType="image/png"
                }
            },
            SocialMediaLinks=new List<SocialMediaLinkDto>
            {
                new SocialMediaLinkDto
                {
                    Url = "https://facebook.com/test-school",
                    SocialPlatformName = "Facebook"
                }
            }
        });
        request.IsError.Should().BeTrue();
    }

    [Fact]
    public void WithNoPhoneNumber_ReturnError()
    {
        var request = CreateSchoolCommand.Create(new CreateSchoolCommandDto(

            "Test Driving School",
            "Test Driving School LLC",
            "Test",
            "Owner",
            "Owner@EMail.com",
            "School@EMail.com",
            "123xad21!~",
            "Test City",
            "123 Test St",
            "",
            "/path/to/valid/image.png",
            "image/png",
            [],
            new WebsiteLinkReadDto
            {
                Url="https://example.com",
                Name="Website",
                Description="Official website",
                IconData=new Domain.Dtos.Image.ImageReadDto
                {
                    Url="/path/to/valid/website-icon.png",
                    ContentType="image/png"
                }
            },
            [
                new SocialMediaLinkDto
                {
                    Url = "https://facebook.com/test-school",
                    SocialPlatformName = "Facebook"
                }
            ]
        ));
        request.IsError.Should().BeTrue();
    }

    [Fact]
    public void WithOnlyOnePhoneNumber_IsSetAsMain()
    {
        var request = CreateSchoolCommand.Create(new CreateSchoolCommandDto(

           "Test Driving School",
           "Test Driving School LLC",
           "Test",
           "Owner",
           "Owner@EMail.com",
           "School@EMail.com",
           "123xad21!~",
           "Test City",
           "123 Test St",
           "123213",
           "/path/to/valid/image.png",
           "image/png",
           [],
           new WebsiteLinkReadDto
           {
               Url="https://example.com",
               Name="Website",
               Description="Official website",
               IconData=new Domain.Dtos.Image.ImageReadDto
               {
                   Url="/path/to/valid/website-icon.png",
                   ContentType="image/png"
               }
           },
           [
               new SocialMediaLinkDto
                {
                    Url = "https://facebook.com/test-school",
                    SocialPlatformName = "Facebook"
                }
           ]
       ));
        request.IsError.Should().BeFalse();
        var returnedValue = request.Value??throw new Exception("The returned value is null");
        returnedValue.PhoneNumber.Should().NotBeNull();
        returnedValue.PhoneNumbersGroups.Should().BeEmpty();
    }

    [Fact]
    public void WithOnePhoneNumberInGroup_SetAsMain_ClearGroup()
    {
        var request = CreateSchoolCommand.Create(new CreateSchoolCommandDto(

           "Test Driving School",
           "Test Driving School LLC",
           "Test",
           "Owner",
           "Owner@EMail.com",
           "School@EMail.com",
           "123xad21!~",
           "Test City",
           "123 Test St",
           "",
           "/path/to/valid/image.png",
           "image/png",
           [new PhoneNumberGroupDto("Default", [new PhoneNumberDto("123213")])],
           new WebsiteLinkReadDto
           {
               Url="https://example.com",
               Name="Website",
               Description="Official website",
               IconData=new Domain.Dtos.Image.ImageReadDto
               {
                   Url="/path/to/valid/website-icon.png",
                   ContentType="image/png"
               }
           },
           [
               new SocialMediaLinkDto
                {
                    Url = "https://facebook.com/test-school",
                    SocialPlatformName = "Facebook"
                }
           ]
       ));
        request.IsError.Should().BeFalse();
        var returnedValue = request.Value??throw new Exception("The returned value is null");
        returnedValue.PhoneNumber.Should().NotBeNull();
        returnedValue.PhoneNumber.Value.Should().Be("123213");
        returnedValue.PhoneNumbersGroups.Count.Should().Be(0);
    }

    [Fact]
    public void WithTwoPhoneNumbers_FirstIsSetAsMain()
    {
        var request = CreateSchoolCommand.Create(new CreateSchoolCommandDto(

           "Test Driving School",
           "Test Driving School LLC",
           "Test",
           "Owner",
           "Owner@EMail.com",
           "School@EMail.com",
           "123xad21!~",
           "Test City",
           "123 Test St",
           "",
           "/path/to/valid/image.png",
           "image/png",
           [new PhoneNumberGroupDto("Default", [new PhoneNumberDto("123213"), new PhoneNumberDto("455656")])],
           new WebsiteLinkReadDto
           {
               Url="https://example.com",
               Name="Website",
               Description="Official website",
               IconData=new Domain.Dtos.Image.ImageReadDto
               {
                   Url="/path/to/valid/website-icon.png",
                   ContentType="image/png"
               }
           },
           [
               new SocialMediaLinkDto
                {
                    Url = "https://facebook.com/test-school",
                    SocialPlatformName = "Facebook"
                }
           ]
       ));
        request.IsError.Should().BeFalse();
        var returnedValue = request.Value??throw new Exception("The returned value is null");
        returnedValue.PhoneNumber.Should().NotBeNull();
        returnedValue.PhoneNumber.Value.Should().Be("123213");
        returnedValue.PhoneNumbersGroups.Count.Should().Be(1);
        returnedValue.PhoneNumbersGroups[0].PhoneNumbers.Count.Should().Be(1);
        returnedValue.PhoneNumbersGroups[0].PhoneNumbers[0].Value.Should().Be("455656");
    }


    [Fact]
    public void WithTwoPhoneNumbers_OneInGroupAndOneInMain_KeepBoth()
    {
        var request = CreateSchoolCommand.Create(new CreateSchoolCommandDto(

           "Test Driving School",
           "Test Driving School LLC",
           "Test",
           "Owner",
           "Owner@EMail.com",
           "School@EMail.com",
           "123xad21!~",
           "Test City",
           "123 Test St",
           "123213",
           "/path/to/valid/image.png",
           "image/png",
           [new PhoneNumberGroupDto("Default", [new PhoneNumberDto("455656")])],
           new WebsiteLinkReadDto
           {
               Url="https://example.com",
               Name="Website",
               Description="Official website",
               IconData=new Domain.Dtos.Image.ImageReadDto
               {
                   Url="/path/to/valid/website-icon.png",
                   ContentType="image/png"
               }
           },
           [
               new SocialMediaLinkDto
                {
                    Url = "https://facebook.com/test-school",
                    SocialPlatformName = "Facebook"
                }
           ]
       ));
        request.IsError.Should().BeFalse();
        var returnedValue = request.Value??throw new Exception("The returned value is null");
        returnedValue.PhoneNumber.Should().NotBeNull();
        returnedValue.PhoneNumber.Value.Should().Be("123213");
        returnedValue.PhoneNumbersGroups.Count.Should().Be(1);
        returnedValue.PhoneNumbersGroups[0].PhoneNumbers.Count.Should().Be(1);
        returnedValue.PhoneNumbersGroups[0].PhoneNumbers[0].Value.Should().Be("455656");
    }
}
