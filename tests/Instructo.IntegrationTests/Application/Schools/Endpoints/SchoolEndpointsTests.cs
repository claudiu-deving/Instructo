using System.Net;
using System.Text;
using System.Text.Json;

using Domain.Dtos;
using Domain.Dtos.Link;
using Domain.Dtos.PhoneNumbers;
using Domain.Dtos.School;
using Domain.Entities;
using Domain.Entities.SchoolEntities;
using Domain.ValueObjects;

using Instructo.IntegrationTests.Infrastructure;

using Microsoft.Extensions.DependencyInjection;

namespace Instructo.IntegrationTests.Application.Schools.Endpoints;

public class SchoolEndpointsTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;

    #region Anonymous Access Tests

    private static JsonSerializerOptions JsonSerializerOptions => new()
    {
        PropertyNameCaseInsensitive=true,
        WriteIndented=true
    };

    [Fact]
    public async Task Get_Schools_Returns_Ok_Without_Authentication()
    {
        // Act
        var response = await _client.GetAsync("/api/schools");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var schools = JsonSerializer.Deserialize<List<SchoolReadDto>>(content, JsonSerializerOptions);

        Assert.NotNull(schools);
    }

    [Fact]
    public async Task Get_School_By_Slug_Returns_Ok_Without_Authentication()
    {
        // Arrange
        var seededSchool = await _seeder!.SeedSchoolWithExplicitDataAsync(new UpdateSchoolCommandDto
        {
            Name="Test School",
            LegalName="Test Company"
        });

        // Act
        var response = await _client.GetAsync($"/api/schools/{seededSchool.Slug}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var school = JsonSerializer.Deserialize<SchoolDetailReadDto>(content, JsonSerializerOptions);
        Assert.NotNull(school);

        Assert.Equal(seededSchool.CompanyName, school.CompanyName);
    }

    #endregion

    #region Create School Tests

    [Fact]
    public async Task Create_School_Without_Authentication_Returns_Unauthorized()
    {
        // Arrange
        var testUser = await _authentificationHelper!.CreateTestUserAsync("Owner");
        var createSchoolDto = CreateValidSchool(testUser);
        var json = JsonSerializer.Serialize(createSchoolDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/schools", content);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Create_School_With_Valid_User_Returns_Created()
    {
        if(_authentificationHelper is null)
        {
            throw new InvalidOperationException("Authentication helper is not initialized.");
        }
        // Arrange
        var testUser = await _authentificationHelper!.CreateTestUserAsync("Owner");
        var token = _authentificationHelper.CreateJwtToken(testUser, "Owner");

        AuthenticationHelper.AddAuthorizationHeader(_client, token);

        var createSchoolDto = CreateValidSchool(testUser);

        var json = JsonSerializer.Serialize(createSchoolDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/schools", content);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdSchool = JsonSerializer.Deserialize<SchoolDetailReadDto>(responseContent, JsonSerializerOptions);

        Assert.NotNull(createdSchool);
        Assert.Equal("New School", createdSchool.Name);
    }

    private static CreateSchoolCommandDto CreateValidSchool(ApplicationUser testUser)
    {
        return new CreateSchoolCommandDto
        {
            Name="New School",
            LegalName="New School LLC",
            SchoolEmail="test@newschool.com",
            PhoneNumber="1234567890",
            OwnerEmail=testUser.Email!,
            City="Cluj-Napoca",
            VechiclesCategories= ["B"],
            CategoryPricings=
                    [
                        new SchoolCategoryPricingDto
                {
                    FullPrice = 2500,
                    InstallmentPrice = 500,
                    Installments = 5,
                    Transmission =  "Manual",
                    VehicleCategory = "B"
                }
                    ],
            BussinessHours= [
                        new BusinessHoursEntryDto
                {
                    DaysOfTheWeek = ["Monday"],
                    Intervals =[new HourIntervals() { StartingHourAndMinute = "08:00", EndingHourAndMinute = "16:00" }]
                },
                ],
            Description="New School Description",
            Address="123 New School St",
            X="45.770439",
            Y="25.591423",
            ImagePath="images/newschool.jpg",
            ImageContentType="image/jpeg",
            Slogan="Learn with us!",
            PhoneNumberGroups=
            [
                new PhoneNumberGroupDto
                {
                    Name = "Main",
                    PhoneNumbers = [new PhoneNumberDto( "1234567890")]
                }
            ],
            ExtraLocations=
            [
                new AddressDto
                {
                    Street = "456 Extra Location St",
                    Latitude = "46.770439",
                    Longitude = "23.591423",
                    AddressType = Domain.Enums.AddressType.LessonStart
                }
            ],
            WebsiteLink=new WebsiteLinkReadDto
            {
                Url="https://www.newschool.com",
                Name="New School Website",
                IconData=new Domain.Dtos.Image.ImageReadDto()
                {
                    FileName="images/newschool-icon.png",
                    ContentType="image/png",
                    Url="New School Icon",
                    Description="New School Icon",
                }
            },
            SocialMediaLinks=
            [
                new SocialMediaLinkDto
                {
                    Url="https://www.facebook.com/newschool",
                    SocialPlatformName="Facebook"
                },
                new SocialMediaLinkDto
                {
                    Url="https://www.instagram.com/newschool",
                    SocialPlatformName="Instagram"
                }
            ],
            Team=new TeamDto
            {
                Instructors=
                [
                    new InstructorDto
                    {
                        FirstName="John",
                        LastName="Doe",
                        ProfileImageUrl="images/johndoe.jpg",
                        ProfileImageContentType="image/jpeg",
                        ProfileImageName = "John Doe",
                        Email = "test@instructor.ro",
                        Gender = "Male",
                        Age = 30,
                        YearsExperience = 5,
                        Categories = [new VehicleCategoryDto( "A","A"),new VehicleCategoryDto("B", "B")],
                        Description = "Experienced driving instructor",
                        PhoneNumber = "1234567890",
                        ProfileImageDescription = "John Doe Profile Image",
                        Specialization = "Advanced Driving Techniques"
                    }
                ]
            }
        };
    }

    #endregion

    #region Update School Tests

    [Fact]
    public async Task Update_School_Without_Authentication_Returns_Unauthorized()
    {
        // Arrange
        var seededSchool = await _seeder!.SeedSchoolWithExplicitDataAsync(new UpdateSchoolCommandDto
        {
            Name="Test School",
            LegalName="Test Company"
        });

        var updateDto = new UpdateSchoolCommandDto
        {
            Name="Updated School Name"
        };

        var json = JsonSerializer.Serialize(updateDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PatchAsync($"/api/schools/{seededSchool.Id}", content);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Update_School_With_Wrong_User_Returns_Forbidden()
    {
        // Arrange

        // Create school owner
        var schoolOwner = await _authentificationHelper!.CreateTestUserAsync("Owner");
        var seededSchool = await _seeder!.SeedSchoolWithExplicitDataAsync(
            new UpdateSchoolCommandDto
            {
                Name="Test School",
                LegalName="Test Company"
            },
            schoolOwner);

        // Create different user trying to update
        var differentUser = await _authentificationHelper!.CreateTestUserAsync("Owner");
        var token = _authentificationHelper.CreateJwtToken(differentUser, "Owner");

        AuthenticationHelper.AddAuthorizationHeader(_client, token);

        var updateDto = new UpdateSchoolCommandDto
        {
            Name="Updated School Name"
        };

        var json = JsonSerializer.Serialize(updateDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PatchAsync($"/api/schools/{seededSchool.Id}", content);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode); // Should be handled by business logic
    }

    [Fact]
    public async Task Update_School_With_Owner_Returns_Ok()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var schoolOwner = await _authentificationHelper!.CreateTestUserAsync("Owner");

        var seededSchool = await _seeder!.SeedSchoolWithExplicitDataAsync(
            new UpdateSchoolCommandDto
            {
                Name="Test School",
                LegalName="Test Company"
            },
            schoolOwner);

        var token = _authentificationHelper.CreateJwtToken(schoolOwner, "Owner");
        AuthenticationHelper.AddAuthorizationHeader(_client, token);

        var updateDto = new UpdateSchoolCommandDto
        {
            Name="Updated School Name"
        };

        var json = JsonSerializer.Serialize(updateDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PatchAsync($"/api/schools/{seededSchool.Id}", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var updatedSchool = JsonSerializer.Deserialize<SchoolDetailReadDto>(responseContent, JsonSerializerOptions);

        Assert.NotNull(updatedSchool);
        Assert.Equal("Updated School Name", updatedSchool.Name);
    }

    [Fact]
    public async Task Update_School_With_IronMan_Returns_Ok()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();

        // Create a school with regular owner
        var schoolOwner = await _authentificationHelper!.CreateTestUserAsync("Owner");
        var seededSchool = await _seeder!.SeedSchoolWithExplicitDataAsync(
            new UpdateSchoolCommandDto
            {
                Name="Test School",
                LegalName="Test Company"
            },
            schoolOwner);

        // Create IronMan user (super admin)
        var ironManUser = await _authentificationHelper.CreateTestUserAsync("IronMan");
        var token = _authentificationHelper.CreateJwtToken(ironManUser, "IronMan");
        AuthenticationHelper.AddAuthorizationHeader(_client, token);

        var updateDto = new UpdateSchoolCommandDto
        {
            Name="IronMan Updated School"
        };

        var json = JsonSerializer.Serialize(updateDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PatchAsync($"/api/schools/{seededSchool.Id}", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    #endregion

    #region Delete School Tests

    [Fact]
    public async Task Delete_School_Without_Authentication_Returns_Unauthorized()
    {
        // Arrange
        var seededSchool = await _seeder!.SeedSchoolWithExplicitDataAsync(new UpdateSchoolCommandDto
        {
            Name="Test School",
            LegalName="Test Company"
        });

        // Act
        var response = await _client.DeleteAsync($"/api/schools/{seededSchool.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Delete_School_With_Owner_Returns_Ok()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var schoolOwner = await _authentificationHelper!.CreateTestUserAsync("Owner");

        var seededSchool = await _seeder!.SeedSchoolWithExplicitDataAsync(
            new UpdateSchoolCommandDto
            {
                Name="Test School",
                LegalName="Test Company"
            },
            schoolOwner);

        var token = _authentificationHelper.CreateJwtToken(schoolOwner, "Owner");
        AuthenticationHelper.AddAuthorizationHeader(_client, token);

        // Act
        var response = await _client.DeleteAsync($"/api/schools/{seededSchool.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    #endregion

    #region Approval Status Tests

    [Fact]
    public async Task Update_Approval_Status_Without_Authentication_Returns_Unauthorized()
    {
        // Arrange
        var seededSchool = await _seeder!.SeedSchoolWithExplicitDataAsync(new UpdateSchoolCommandDto
        {
            Name="Test School",
            LegalName="Test Company"
        });

        var approvalDto = new UpdateApprovalStatusCommandDto
        {
            IsApproved=true
        };

        var json = JsonSerializer.Serialize(approvalDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PatchAsync($"/api/schools/approval/{seededSchool.Id}", content);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Update_Approval_Status_With_Owner_Returns_Forbidden()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var schoolOwner = await _authentificationHelper!.CreateTestUserAsync("Owner");

        var seededSchool = await _seeder!.SeedSchoolWithExplicitDataAsync(
            new UpdateSchoolCommandDto
            {
                Name="Test School",
                LegalName="Test Company"
            },
            schoolOwner);

        var token = _authentificationHelper.CreateJwtToken(schoolOwner, "Owner");
        AuthenticationHelper.AddAuthorizationHeader(_client, token);

        var approvalDto = new UpdateApprovalStatusCommandDto
        {
            IsApproved=true
        };

        var json = JsonSerializer.Serialize(approvalDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PatchAsync($"/api/schools/approval/{seededSchool.Id}", content);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Update_Approval_Status_With_IronMan_Returns_Ok()
    {
        // Arrange
        var ironManUser = await _authentificationHelper!.CreateTestUserAsync("IronMan");

        var seededSchool = await _seeder!.SeedSchoolWithExplicitDataAsync(new UpdateSchoolCommandDto
        {
            Name="Test School",
            LegalName="Test Company"
        });

        var token = _authentificationHelper.CreateJwtToken(ironManUser, "IronMan");
        AuthenticationHelper.AddAuthorizationHeader(_client, token);

        var approvalDto = new UpdateApprovalStatusCommandDto
        {
            IsApproved=true
        };

        var json = JsonSerializer.Serialize(approvalDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PatchAsync($"/api/schools/approval/{seededSchool.Id}", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    #endregion

    #region Validation Tests

    [Fact]
    public async Task Get_School_By_Invalid_Slug_Returns_NotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/schools/non-existent-slug");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_NonExistent_School_Returns_NotFound()
    {
        // Arrange
        var testUser = await _authentificationHelper!.CreateTestUserAsync("Owner");
        var token = _authentificationHelper.CreateJwtToken(testUser, "Owner");

        AuthenticationHelper.AddAuthorizationHeader(_client, token);

        var updateDto = new UpdateSchoolCommandDto
        {
            Name="Updated Name"
        };

        var json = JsonSerializer.Serialize(updateDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PatchAsync($"/api/schools/{Guid.NewGuid()}", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode); // Business logic should handle this
    }

    #endregion

    #region Helper Tests

    [Fact]
    public async Task CreateSchoolThroughSeeder_Should_Work()
    {
        // Act
        var school = await _seeder!.SeedSchoolWithExplicitDataAsync(new UpdateSchoolCommandDto
        {
            Name="Test School",
            LegalName="Test Company"
        });

        // Assert
        Assert.NotNull(school);
        Assert.Equal("Test School", school.Name);
        Assert.Equal("Test Company", school.CompanyName);
    }

    [Fact]
    public async Task GetAllSchools_Should_Return_AtLeast_One_School()
    {
        // Act
        await _seeder!.SeedSchoolsAsync(5);
        // Assert

        var response = await _client.GetAsync("/api/schools");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<List<SchoolReadDto>>(content, JsonSerializerOptions);

        Assert.NotNull(result);
        Assert.All(result, school => Assert.NotNull(school.Name));
    }

    #endregion
}
