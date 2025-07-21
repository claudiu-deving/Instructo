using System.Threading.Tasks;

using Domain.Dtos.School;
using Domain.Entities.SchoolEntities;
using Domain.ValueObjects;

using Infrastructure.Data.Repositories.Queries;

using Instructo.IntegrationTests.Data.Repositories.Queries;

using Microsoft.EntityFrameworkCore;

using Xunit.Abstractions;

namespace Instructo.IntegrationTests.Infrastructure;

[Collection("Integration Tests")]
public class DetailedSchoolQueriesRepositoryTests(
    IntegrationTestFixture integrationTestFixture,
    ITestOutputHelper testOutputHelper) : IntegrationTestBase(integrationTestFixture)
{
    private readonly SchoolQueriesRepository _sut = new SchoolQueriesRepository(
        integrationTestFixture.GetDbContext(),
        new TestLogger(testOutputHelper));

    [Fact]
    public async Task WhenConnectionIsTried_ItConnects()
    {
        var result = await _sut.GetAllDetailedAsync();
        Assert.NotNull(result);
        Assert.False(result.IsError);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count());
    }

    [Fact]
    public async Task GetAllDetailedAsync_ReturnsSchoolDtosWithCorrectBasicProperties()
    {
        var result = await _sut.GetAllDetailedAsync();

        Assert.False(result.IsError);
        var schools = result.Value.ToList();

        var eliteSchool = schools.FirstOrDefault(s => s.Name=="Elite Driving Academy");
        Assert.NotNull(eliteSchool);
        Assert.Equal(Guid.Parse("55555555-5555-5555-5555-555555555555"), eliteSchool.Id);
        Assert.Equal("Elite Driving Academy SRL", eliteSchool.CompanyName);
        Assert.Equal("contact@elitedriving.ro", eliteSchool.Email);
        Assert.Equal("elite-driving-academy-srl", eliteSchool.Slug);
        Assert.Equal("+40721123456", eliteSchool.PhoneNumber);
        Assert.Equal("Learn to drive with confidence!", eliteSchool.Slogan);
        Assert.Equal("Professional driving school with experienced instructors and modern vehicles.", eliteSchool.Description);
        Assert.Equal("Aurel Vlaicu", eliteSchool.StreetAndNumber);
        Assert.NotEqual(0, eliteSchool.Longitude);
        Assert.NotEqual(0, eliteSchool.Latitude);

        var professionalSchool = schools.FirstOrDefault(s => s.Name=="Professional Drive Center");
        Assert.NotNull(professionalSchool);
        Assert.Equal(Guid.Parse("66666666-6666-6666-6666-666666666666"), professionalSchool.Id);
        Assert.Equal("Professional Drive Center SRL", professionalSchool.CompanyName);
        Assert.Equal("info@profdrive.ro", professionalSchool.Email);
        Assert.Equal("professional-drive-center-srl", professionalSchool.Slug);
        Assert.Equal("+40722987654", professionalSchool.PhoneNumber);
        Assert.Equal("Your path to safe driving!", professionalSchool.Slogan);
        Assert.Equal("Comprehensive driving education with focus on road safety and practical skills.", professionalSchool.Description);
    }

    [Fact]
    public async Task GetAllDetailedAsync_ReturnsSchoolDtosWithCorrectNestedObjects()
    {
        var result = await _sut.GetAllDetailedAsync();

        Assert.False(result.IsError);
        var schools = result.Value.ToList();

        var eliteSchool = schools.FirstOrDefault(s => s.Name=="Elite Driving Academy");
        Assert.NotNull(eliteSchool);

        Assert.NotNull(eliteSchool.IconData);
        Assert.Equal("school1-logo.png", eliteSchool.IconData.FileName);
        Assert.Equal("image/png", eliteSchool.IconData.ContentType);
        Assert.Equal("https://example.com/images/school1-logo.png", eliteSchool.IconData.Url);

        Assert.NotNull(eliteSchool.CityName);
        Assert.NotNull(eliteSchool.CountyId);

        var professionalSchool = schools.FirstOrDefault(s => s.Name=="Professional Drive Center");
        Assert.NotNull(professionalSchool);
        Assert.NotNull(professionalSchool.IconData);
        Assert.Equal("school2-logo.jpg", professionalSchool.IconData.FileName);
        Assert.Equal("image/jpeg", professionalSchool.IconData.ContentType);
        Assert.Equal("https://example.com/images/school2-logo.jpg", professionalSchool.IconData.Url);
    }

    [Fact]
    public async Task GetAllDetailedAsync_ReturnsSchoolDtosWithCorrectPhoneNumbersAndBusinessHours()
    {
        var result = await _sut.GetAllDetailedAsync();

        Assert.False(result.IsError);
        var schools = result.Value.ToList();

        var eliteSchool = schools.FirstOrDefault(s => s.Name=="Elite Driving Academy");
        Assert.NotNull(eliteSchool);

        Assert.NotNull(eliteSchool.PhoneNumbersGroups);
        var phoneGroups = eliteSchool.PhoneNumbersGroups.ToList();
        Assert.Single(phoneGroups);
        Assert.Equal("Main Office", phoneGroups[0].Name);
        Assert.Single(phoneGroups[0].PhoneNumbers);
        Assert.Equal("+40721123456", phoneGroups[0].PhoneNumbers.First().Value);

        Assert.NotNull(eliteSchool.BussinessHours);
        Assert.NotEmpty(eliteSchool.BussinessHours);

        var mondayToThursday = eliteSchool.BussinessHours
            .FirstOrDefault(e => e.DayOfTheWeek.Contains(DayOfWeek.Monday));
        Assert.NotNull(mondayToThursday);
        Assert.Equal(4, mondayToThursday.DayOfTheWeek.Count);
        Assert.Single(mondayToThursday.Times);
        Assert.Equal(8, mondayToThursday.Times[0].Start.Hour);
        Assert.Equal(0, mondayToThursday.Times[0].Start.Minutes);
        Assert.Equal(18, mondayToThursday.Times[0].End.Hour);
        Assert.Equal(0, mondayToThursday.Times[0].End.Minutes);

        var friday = eliteSchool.BussinessHours
            .FirstOrDefault(e => e.DayOfTheWeek.Contains(DayOfWeek.Friday));
        Assert.NotNull(friday);
        Assert.Single(friday.DayOfTheWeek);
        Assert.Equal(8, friday.Times[0].Start.Hour);
        Assert.Equal(17, friday.Times[0].End.Hour);

        var saturday = eliteSchool.BussinessHours
            .FirstOrDefault(e => e.DayOfTheWeek.Contains(DayOfWeek.Saturday));
        Assert.NotNull(saturday);
        Assert.Single(saturday.DayOfTheWeek);
        Assert.Equal(9, saturday.Times[0].Start.Hour);
        Assert.Equal(14, saturday.Times[0].End.Hour);
    }

    [Fact]
    public async Task GetAllDetailedAsync_ReturnsSchoolDtosWithCorrectCollections()
    {
        var result = await _sut.GetAllDetailedAsync();

        Assert.False(result.IsError);
        var schools = result.Value!.ToList();

        var eliteSchool = schools.FirstOrDefault(s => s.Name=="Elite Driving Academy");
        Assert.NotNull(eliteSchool);
        Assert.NotNull(eliteSchool.VehicleCategories);
        Assert.Equal(2, eliteSchool.VehicleCategories.Count);

        var eliteCategories = eliteSchool.VehicleCategories.ToList();
        Assert.Contains(eliteCategories, vc => vc.Name=="A");
        Assert.Contains(eliteCategories, vc => vc.Name=="BE");
        Assert.Contains(eliteCategories, vc => vc.Description.Contains("Motorcycles"));
        Assert.Contains(eliteCategories, vc => vc.Description.Contains("Vehicle-trailer combinations exceeding 4,250kg total"));

        Assert.NotNull(eliteSchool.Links);
        Assert.Empty(eliteSchool.Links);

        Assert.NotNull(eliteSchool.ArrCertificates);
        Assert.Empty(eliteSchool.ArrCertificates);

        var professionalSchool = schools.FirstOrDefault(s => s.Name=="Professional Drive Center");
        Assert.NotNull(professionalSchool);
        Assert.NotNull(professionalSchool.VehicleCategories);
        Assert.Single(professionalSchool.VehicleCategories);

        var professionalCategory = professionalSchool.VehicleCategories.First();
        Assert.Equal("BE", professionalCategory.Name);
        Assert.Contains("Vehicle-trailer combinations", professionalCategory.Description);
    }

    [Fact]
    public async Task GetBySlugAsync_ReturnsCorrectSchoolEntityBySlug()
    {
        var result = await _sut.GetBySlugAsync("elite-driving-academy-srl");

        Assert.False(result.IsError);
        Assert.NotNull(result.Value);
        Assert.Equal("Elite Driving Academy", result.Value.Name);
        Assert.Equal("elite-driving-academy-srl", result.Value.Slug);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectSchoolEntityById()
    {
        var schoolId = SchoolId.CreateNew(Guid.Parse("55555555-5555-5555-5555-555555555555"));
        var result = await _sut.GetByIdAsync(schoolId);

        Assert.False(result.IsError);
        Assert.NotNull(result.Value);
        Assert.Equal("Elite Driving Academy", result.Value.Name);
        Assert.Equal(schoolId.Id, result.Value.Id);
    }

    [Fact]
    public async Task GetAllDetailedAsync_ReturnsConsistentDataAcrossMultipleCalls()
    {
        var result1 = await _sut.GetAllDetailedAsync();
        var result2 = await _sut.GetAllDetailedAsync();

        Assert.False(result1.IsError);
        Assert.False(result2.IsError);

        var schools1 = result1.Value.ToList();
        var schools2 = result2.Value.ToList();

        Assert.Equal(schools1.Count, schools2.Count);

        var eliteSchool1 = schools1.First(s => s.Id==Guid.Parse("55555555-5555-5555-5555-555555555555"));
        var eliteSchool2 = schools2.First(s => s.Id==Guid.Parse("55555555-5555-5555-5555-555555555555"));

        Assert.Equal(eliteSchool1.Name, eliteSchool2.Name);
        Assert.Equal(eliteSchool1.CompanyName, eliteSchool2.CompanyName);
        Assert.Equal(eliteSchool1.Email, eliteSchool2.Email);
    }
}