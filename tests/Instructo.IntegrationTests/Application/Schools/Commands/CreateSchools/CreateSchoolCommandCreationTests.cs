using Application.Schools.Commands.CreateSchool;

using Domain.Dtos;
using Domain.Dtos.Image;
using Domain.Dtos.Link;
using Domain.Dtos.PhoneNumbers;
using Domain.Dtos.School;
using Domain.ValueObjects;

using JetBrains.Annotations;

namespace Instructo.UnitTests.Application.Schools.Commands.CreateSchools;

public class CreateSchoolCommandCreationTests
{
    [Fact]
    public void ValidInputShouldCreateANewCommand()
    {
        var request = CreateSchoolCommand.Create(new CreateSchoolCommandDto(
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
           ArrCertifications: []
        ));
        if(request.IsError)
            throw new InvalidOperationException(
                $"Failed to create valid command: {string.Join(", ", request.Errors.Select(e => e.Message))}");
        var command = request.Value!;
        Assert.NotNull(command);
        Assert.Equal("Test Driving School", command.Name);
        Assert.Equal("Test Driving School LLC", command.LegalName);
    }

    [Fact]
    public void LegalNameStartsWithNonCapitalLetter_InvalidatesInput()
    {
        var request = CreateSchoolCommand.Create(new CreateSchoolCommandDto(
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
          ArrCertifications: []
       ));
        Assert.True(request.IsError);
        Assert.Contains("Company name must start with a capital letter", request.Errors.Select(e => e.Code));
    }

    [Fact]
    public void WithNoPhoneNumber_ReturnError()
    {
        var request = CreateSchoolCommand.Create(new CreateSchoolCommandDto(
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
          ArrCertifications: []
       ));
        Assert.True(request.IsError);
        Assert.Contains("No phone number groups and no phone number", request.Errors.Select(e => e.Message));
    }

    [Fact]
    public void WithOnlyOnePhoneNumber_IsSetAsMain()
    {
        var request = CreateSchoolCommand.Create(new CreateSchoolCommandDto(
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
          PhoneNumberGroups: [new PhoneNumberGroupDto("Test", [new PhoneNumberDto("0758154581")])],
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
          ArrCertifications: []
       ));
        Assert.False(request.IsError);
        var returnedValue = request.Value??throw new Exception("The returned value is null");
        Assert.NotNull(returnedValue.PhoneNumber);
        Assert.Empty(returnedValue.PhoneNumbersGroups);
    }



    [Fact]
    public void WithTwoPhoneNumbers_FirstIsSetAsMain()
    {
        var request = CreateSchoolCommand.Create(new CreateSchoolCommandDto(
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
          PhoneNumberGroups:
          [new PhoneNumberGroupDto("Test", [new PhoneNumberDto("0758154581")]),
              new PhoneNumberGroupDto("Test2", [new PhoneNumberDto("0758154581")])],
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
          ArrCertifications: []
       ));
        Assert.False(request.IsError);
        var returnedValue = request.Value??throw new Exception("The returned value is null");
        Assert.NotNull(returnedValue.PhoneNumber);
        Assert.Equal("0758154581", returnedValue.PhoneNumber.Value);
        Assert.Single(returnedValue.PhoneNumbersGroups);
        Assert.Single(returnedValue.PhoneNumbersGroups[0].PhoneNumbers);
        Assert.Equal("0758154581", returnedValue.PhoneNumbersGroups[0].PhoneNumbers[0].Value);
    }


    [Fact]
    public void WithTwoPhoneNumbers_OneInGroupAndOneInMain_KeepBoth()
    {
        var request = CreateSchoolCommand.Create(new CreateSchoolCommandDto(
             Name: "Test Driving School",
             LegalName: "Test Driving School LLC",
             OwnerEmail: "Owner@EMail.com",
             SchoolEmail: "Scgool@Email.com",
             City: "Test City",
             Address: "123 Test St",
             PhoneNumber: "075751512",
             ImagePath: "/path/to/valid/image.png",
             ImageContentType: "image/png",
             Slogan: "slogan",
             Description: "description",
             X: "25,251",
             Y: "42.81",
             PhoneNumberGroups:
             [new PhoneNumberGroupDto("Test", [new PhoneNumberDto("0758154581")])],
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
             ArrCertifications: []
          ));
        Assert.False(request.IsError);
        var returnedValue = request.Value??throw new Exception("The returned value is null");
        Assert.NotNull(returnedValue.PhoneNumber);
        Assert.Equal("075751512", returnedValue.PhoneNumber.Value);
        Assert.Single(returnedValue.PhoneNumbersGroups);
        Assert.Single(returnedValue.PhoneNumbersGroups[0].PhoneNumbers);
        Assert.Equal("0758154581", returnedValue.PhoneNumbersGroups[0].PhoneNumbers[0].Value);
    }
}