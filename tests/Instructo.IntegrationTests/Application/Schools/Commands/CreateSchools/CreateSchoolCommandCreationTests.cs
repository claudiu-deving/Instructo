using Application.Schools.Commands.CreateSchool;

using Domain.Dtos;
using Domain.Dtos.Image;
using Domain.Dtos.Link;
using Domain.Dtos.PhoneNumbers;
using Domain.Dtos.School;
using Domain.ValueObjects;

namespace Instructo.IntegrationTests.Application.Schools.Commands.CreateSchools;

public class CreateSchoolCommandCreationTests
{
    [Fact]
    public void ValidInputShouldCreateANewCommand()
    {
        var dto = new CreateSchoolCommandDto(
           Name: "Test Driving School",
           LegalName: "Test Driving School LLC",
           OwnerEmail: "Owner@EMail.com",
           SchoolEmail: "Scgool@Email.com",
           City: "Test City",
           Address: "123 Test St",
           PhoneNumber: "123-456-7890",
           ImagePath: "/path/to/valid/image.png",
           ImageContentType: "image/png",
           Slogan: "slogan",
           Description: "description",
           X: "25,251",
           Y: "42.81",
           PhoneNumberGroups: [],
           WebsiteLink: new WebsiteLinkReadDto("https://example.com", "Website", "Official website",
                new ImageReadDto
                (
                    "",
                    "/path/to/valid/website-icon.png",
                    "image/png",
                    "Some description"
                )
            ),
           SocialMediaLinks: [new SocialMediaLinkDto("https://facebook.com/test-school", "Facebook")],
           BussinessHours: [new BusinessHoursEntryDto([DayOfWeek.Monday.ToString()], [new HourIntervals("08:00", "17:00")])],
           VechiclesCategories: ["B"],
           ArrCertifications: [],
           NumberOfStudents: 1002,
              CategoryPricings: [],
              ExtraLocations: [],
              Team: null
        );
        var request = CreateSchoolCommand.Create(dto);
        if(request.IsError)
            throw new InvalidOperationException(
                $"Failed to create valid command: {string.Join(", ", request.Errors.Select(e => e.Message))}");
        var command = request.Value!;
        Assert.NotNull(command);
        Assert.Equal(dto.Name, command.Name);
        Assert.Equal(dto.LegalName, command.LegalName);
    }

    [Fact]
    public void LegalNameStartsWithNonCapitalLetter_InvalidatesInput()
    {
        var dto = new CreateSchoolCommandDto(
          Name: "Test Driving School",
          LegalName: "test Driving School LLC",
          OwnerEmail: "Owner@EMail.com",
          SchoolEmail: "Scgool@Email.com",
          City: "Test City",
          Address: "123 Test St",
          PhoneNumber: "123-456-7890",
          ImagePath: "/path/to/valid/image.png",
          ImageContentType: "image/png",
          Slogan: "slogan",
          Description: "description",
          X: "25,251",
          Y: "42.81",
          PhoneNumberGroups: [],
          WebsiteLink: new WebsiteLinkReadDto("https://example.com", "Website", "Official website",
               new ImageReadDto
               (
                   "",
                   "/path/to/valid/website-icon.png",
                   "image/png",
                   "Some description"
               )
           ),
          SocialMediaLinks: [new SocialMediaLinkDto("https://facebook.com/test-school", "Facebook")],
          BussinessHours: [new BusinessHoursEntryDto([DayOfWeek.Monday.ToString()], [new HourIntervals("08:00", "17:00")])],
          VechiclesCategories: ["B"],
          ArrCertifications: [],
           NumberOfStudents: 1002,
              CategoryPricings: [],
              ExtraLocations: [],
              Team: null
       );
        var request = CreateSchoolCommand.Create(dto);
        Assert.True(request.IsError);
        Assert.Contains("Company name must start with a capital letter", request.Errors.Select(e => e.Code));
    }

    [Fact]
    public void WithNoPhoneNumber_ReturnError()
    {
        var dto = new CreateSchoolCommandDto(
          Name: "Test Driving School",
          LegalName: "Test Driving School LLC",
          OwnerEmail: "Owner@EMail.com",
          SchoolEmail: "Scgool@Email.com",
          City: "Test City",
          Address: "123 Test St",
          PhoneNumber: null,
          ImagePath: "/path/to/valid/image.png",
          ImageContentType: "image/png",
          Slogan: "slogan",
          Description: "description",
          X: "25,251",
          Y: "42.81",
          PhoneNumberGroups: [],
          WebsiteLink: new WebsiteLinkReadDto("https://example.com", "Website", "Official website",
               new ImageReadDto
               (
                   "",
                   "/path/to/valid/website-icon.png",
                   "image/png",
                   "Some description"
               )
           ),
          SocialMediaLinks: [new SocialMediaLinkDto("https://facebook.com/test-school", "Facebook")],
          BussinessHours: [new BusinessHoursEntryDto([DayOfWeek.Monday.ToString()], [new HourIntervals("08:00", "17:00")])],
          VechiclesCategories: ["B"],
          ArrCertifications: [],
           NumberOfStudents: 1002,
              CategoryPricings: [],
              ExtraLocations: [],
              Team: null
       );
        var request = CreateSchoolCommand.Create(dto);
        Assert.True(request.IsError);
        var messages = request.Errors.Select(e => e.Message).ToList();
        Assert.Contains("Phone number cannot be empty", messages);
    }


}