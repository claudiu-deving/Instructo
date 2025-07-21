using System.Threading.Tasks;

using Bogus;

using Domain.Dtos;
using Domain.Entities;
using Domain.Entities.SchoolEntities;
using Domain.Enums;
using Domain.Mappers;
using Domain.Models;
using Domain.Shared;
using Domain.ValueObjects;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NetTopologySuite;
using NetTopologySuite.Geometries;

using Image = Domain.Entities.Image;

namespace Infrastructure.Data;

public class SchoolSeeder
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<SchoolSeeder> _logger;
    private readonly Faker _faker;
    private readonly GeometryFactory _geometryFactory;

    // Romanian geographical boundaries
    private const double MIN_LATITUDE = 43.6;
    private const double MAX_LATITUDE = 48.3;
    private const double MIN_LONGITUDE = 20.3;
    private const double MAX_LONGITUDE = 29.7;

    private readonly string[] _slogans = [
        "Learn to drive with confidence!",
        "Your path to safe driving!",
        "Professional driving education since {year}",
        "Where safety meets excellence",
        "Driving skills for life",
        "Your journey to independence starts here",
        "Safe drivers, safer roads",
        "Excellence in driving education",
        "Building confident drivers",
        "The road to success starts here"
    ];

    public SchoolSeeder(AppDbContext context, UserManager<ApplicationUser> userManager, ILogger<SchoolSeeder> logger)
    {
        _context=context;
        _userManager=userManager;
        _logger=logger;
        _faker=new Faker("ro");
        _geometryFactory=NtsGeometryServices.Instance.CreateGeometryFactory(4326);
    }

    public static SchoolSeeder Create(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<AppDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var logger = serviceProvider.GetRequiredService<ILogger<SchoolSeeder>>();

        return new SchoolSeeder(context, userManager, logger);
    }

    /// <summary>
    /// Seeds the specified number of schools with realistic data
    /// </summary>
    /// <param name="count">Number of schools to create</param>
    /// <param name="includeTestData">Whether to include predefined test data</param>
    public async Task SeedSchoolsAsync(int count = 5, bool includeTestData = false)
    {
        try
        {
            _logger.LogInformation("Starting school seeding process for {count} schools", count);

            // Ensure we have required reference data
            await EnsureReferenceDataAsync();

            var existingSchoolsCount = await _context.Schools.CountAsync();
            if(existingSchoolsCount>=count&&!includeTestData)
            {
                _logger.LogInformation("Database already contains {existingCount} schools, skipping seeding", existingSchoolsCount);
                return;
            }

            // Create owner users first
            var owners = await CreateOwnerUsersAsync(count);

            // Get reference data
            var cities = await _context.Cities.Include(c => c.County).ToListAsync();
            var vehicleCategories = await _context.Categories.ToListAsync();
            var certificates = await _context.CertificateTypes.ToListAsync();

            if(!cities.Any())
            {
                _logger.LogWarning("No cities found in database. Creating sample cities.");
                cities=await CreateSampleCitiesAsync();
            }

            if(!vehicleCategories.Any())
            {
                _logger.LogWarning("No vehicle categories found in database. This may cause issues.");
            }

            // Create schools
            var schools = new List<School>();
            for(int i = 0; i<count; i++)
            {
                try
                {
                    var school = await CreateSchoolAsync(owners[i], cities, vehicleCategories, certificates);
                    schools.Add(school);
                    _logger.LogDebug("Created school: {schoolName}", school.Name);
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Failed to create school {index}", i+1);
                }
            }



            // Save all schools
            if(schools.Any())
            {
                _context.Schools.AddRange(schools);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully seeded {count} schools", schools.Count);
            }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to seed schools");
            throw;
        }
    }

    private async Task<List<ApplicationUser>> CreateOwnerUsersAsync(int count)
    {
        var owners = new List<ApplicationUser>();

        for(int i = 0; i<count; i++)
        {
            var user = new ApplicationUser
            {
                Id=Guid.NewGuid(),
                FirstName=_faker.Name.FirstName(),
                LastName=_faker.Name.LastName(),
                Email=_faker.Internet.Email(),
                EmailConfirmed=true,
                IsActive=true,
                Created=_faker.Date.PastOffset(2).DateTime,
                PhoneNumber=_faker.Phone.PhoneNumber()
            };

            user.UserName=user.Email;
            user.NormalizedEmail=user.Email.ToUpperInvariant();
            user.NormalizedUserName=user.Email.ToUpperInvariant();

            var result = await _userManager.CreateAsync(user, "TempPassword123!");
            if(result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Owner");
                owners.Add(user);
                _logger.LogDebug("Created owner user: {email}", user.Email);
            }
            else
            {
                _logger.LogWarning("Failed to create user {email}: {errors}",
                    user.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        return owners;
    }



    private async Task<School> CreateSchoolAsync(
        ApplicationUser owner,
        List<City> cities,
        List<VehicleCategory> vehicleCategories,
        List<ArrCertificate> certificates)
    {
        _faker.Locale="ro-RO";
        // Generate school data
        var schoolNameTemplate = _faker.Company.CompanyName();
        var cityName = _faker.PickRandom(_context.Cities);
        var schoolName = $"{schoolNameTemplate}";
        var companyName = $"{schoolName} SRL";

        // Create value objects
        var name = SchoolName.Create(schoolName).ValueOrThrow();
        var legalName = LegalName.Create(companyName).ValueOrThrow();
        var email = Email.Create(_faker.Internet.Email(provider: "driving-school.ro")).ValueOrThrow();
        var slogan = Slogan.Create(_faker.PickRandom(_slogans).Replace("{year}", _faker.Date.Past(10).Year.ToString())).ValueOrThrow();
        var description = Description.Create(_faker.Lorem.Paragraph(3)).ValueOrThrow();

        // Create phone number and groups
        var phoneNumber = PhoneNumber.Create(_faker.Phone.PhoneNumber()).ValueOrThrow();
        var phoneNumberGroups = new List<PhoneNumbersGroup>
        {
            new PhoneNumbersGroup
            {
                Name = "Main Office",
                PhoneNumbers = [new PhoneNumber { Value = phoneNumber.Value }]
            }
        };

        // Create business hours
        var businessHours = CreateBusinessHours();

        // Create address
        var address = CreateRandomAddress();

        // Select random city
        var city = _faker.PickRandom(cities);

        // Create statistics
        var statistics = new Statistics
        {
            NumberOfStudents=_faker.Random.Int(50, 500)
        };

        // Select random vehicle categories (1-3 categories)
        var selectedCategories = _faker.PickRandom(vehicleCategories, _faker.Random.Int(1, Math.Min(3, vehicleCategories.Count))).ToList();

        // Select random certificates (0-2 certificates)
        var selectedCertificates = _faker.PickRandom(certificates, _faker.Random.Int(0, Math.Min(2, certificates.Count))).ToList();

        // Create category pricings
        var categoryPricings = selectedCategories.Select(vc => new SchoolCategoryPricing
        {
            VehicleCategoryId=vc.Id,
            FullPrice=_faker.Random.Decimal(1000, 3000),
            Installments=_faker.Random.Int(1, 6),
            InstallmentPrice=_faker.Random.Decimal(200, 600)
        }).ToList();

        // Create team
        var team = Team.Create(Guid.NewGuid()); // Will be updated with actual school ID after creation

        for(int i = 0; i<_faker.Random.Int(1, 30); i++)
        {
            var instructor = InstructorProfile.Create(
                _faker.Name.FirstName(),
                _faker.Name.LastName(),
                _faker.Random.Int(1980, 2000),
                _faker.Random.Int(1, 20),
                _faker.PickRandom(new[] { "Car", "Motorcycle", "Truck" }),
                _faker.Lorem.Sentence(10),
                _faker.Phone.PhoneNumber(),
                _faker.Internet.Email(),
                _faker.PickRandom(new string[] { "female", "male" }),
              await CreateRandomImage(),
                _faker.PickRandom(vehicleCategories, _faker.Random.Int(1, 3)).ToList()
                );
            team.AddInstructor(instructor);
        }

        // Create school icon (optional)
        Image? icon = await CreateRandomImage();


        // Create the school
        var school = School.Create(
            owner,
            name,
            legalName,
            email,
            phoneNumber,
            phoneNumberGroups,
            businessHours,
            selectedCategories,
            selectedCertificates,
            icon,
            city,
            slogan,
            description,
            address,
            statistics,
            team
        );
        school.SetCategoryPricings(categoryPricings);

        for(int i = 0; i<_faker.Random.Int(1, 3); i++)
        {
            var extraLocation = CreateRandomAddress(AddressType.LessonStart);
            school.AddExtraLocation(extraLocation);
        }

        for(int i = 0; i<_faker.Random.Int(1, 3); i++)
        {
            var randomLink = await CreateRandomSocialMediaLink();
            school.AddLink(randomLink);
        }

        return school;
    }

    private async Task<WebsiteLink> CreateRandomSocialMediaLink()
    {
        var socialMediaPlatforms = new[] { "Facebook", "Instagram", "Twitter", "LinkedIn", "YouTube" };
        var platform = _faker.PickRandom(socialMediaPlatforms);
        var description = _faker.Lorem.Sentence(5);
        var url = _faker.Internet.UrlWithPath();
        var image = await CreateRandomImage();
        return WebsiteLink.Create(platform, url, description, image).ValueOrThrow();
    }

    private BussinessHours CreateBusinessHours()
    {
        var entries = new List<BusinessHoursEntryDto>
        {
            new BusinessHoursEntryDto
            {
                DaysOfTheWeek = [(DayOfWeek.Monday).ToString(),
                    ( DayOfWeek.Tuesday).ToString(),
                    ( DayOfWeek.Wednesday).ToString(),
                    ( DayOfWeek.Thursday).ToString()],
                Intervals = [new HourIntervals
                {
                    StartingHourAndMinute = new TimeOfDay( 8,  0 ).ToString(),
                    EndingHourAndMinute = new TimeOfDay( 18,  0 ).ToString()
                }]
            },

        };

        return BussinessHours.Create(entries).ValueOrThrow();
    }

    private Address CreateRandomAddress(AddressType addressType = AddressType.MainLocation)
    {
        var streetName = _faker.Address.StreetName();
        var streetNumber = _faker.Random.Int(1, 200);
        var street = $"{streetName} {streetNumber}";

        var latitude = _faker.Random.Double(MIN_LATITUDE, MAX_LATITUDE);
        var longitude = _faker.Random.Double(MIN_LONGITUDE, MAX_LONGITUDE);

        return Address.Create(street, latitude.ToString("F6"), longitude.ToString("F6"), addressType);
    }



    private async Task<Image?> CreateRandomImage()
    {
        var result = Image.Create(_faker.Image.PicsumUrl(width: 200, height: 200, false),
             "image/jpeg",
             _faker.Internet.UrlWithPath(),
             "School icon image")
             .OnSuccess(image => _context.Images.Add(image))
             .OnError(errors => _logger.LogWarning("Failed to create school icon: {errors}", string.Join(", ", errors.Select(e => e.Message))));


        return result.Value!;
    }

    private async Task<List<City>> CreateSampleCitiesAsync()
    {
        // Create sample counties first
        var counties = new List<County>
        {
            new County { Id = 1, Name = "Bucuresti", Code = "B" },
            new County { Id = 2, Name = "Cluj", Code = "CJ" },
            new County { Id = 3, Name = "Timis", Code = "TM" }
        };

        _context.Counties.AddRange(counties);

        // Create sample cities
        var cities = new List<City>
        {
            new City { Id = 1, Name = "București", County = counties[0] },
            new City { Id = 2, Name = "Cluj-Napoca", County = counties[1] },
            new City { Id = 3, Name = "Timișoara", County = counties[2] }
        };

        _context.Cities.AddRange(cities);
        await _context.SaveChangesAsync();

        return cities;
    }

    private async Task EnsureReferenceDataAsync()
    {
        // Check if we have required reference data, if not log warnings
        var citiesCount = await _context.Cities.CountAsync();
        var vehicleCategoriesCount = await _context.Categories.CountAsync();

        if(citiesCount==0)
        {
            _logger.LogWarning("No cities found in database. Sample cities will be created.");
        }

        if(vehicleCategoriesCount==0)
        {
            _logger.LogWarning("No vehicle categories found in database. Schools will be created without categories.");
        }
    }

    /// <summary>
    /// Creates a single school with specified parameters for testing
    /// </summary>
    public async Task<School> CreateSingleSchoolAsync(
        string schoolName,
        string ownerEmail,
        string ownerFirstName,
        string ownerLastName)
    {
        // Create owner
        var owner = new ApplicationUser
        {
            Id=Guid.NewGuid(),
            FirstName=ownerFirstName,
            LastName=ownerLastName,
            Email=ownerEmail,
            UserName=ownerEmail,
            EmailConfirmed=true,
            IsActive=true,
            Created=DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(owner, "TempPassword123!");
        if(!result.Succeeded)
        {
            throw new InvalidOperationException($"Failed to create owner user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        await _userManager.AddToRoleAsync(owner, "Owner");

        // Get reference data
        var cities = await _context.Cities.Include(c => c.County).ToListAsync();
        var vehicleCategories = await _context.Categories.ToListAsync();
        var certificates = await _context.CertificateTypes.ToListAsync();

        if(!cities.Any())
        {
            cities=await CreateSampleCitiesAsync();
        }

        // Create school with specified name
        var name = SchoolName.Create(schoolName).ValueOrThrow();
        var legalName = LegalName.Create($"{schoolName} SRL").ValueOrThrow();
        var email = Email.Create(_faker.Internet.Email(provider: "driving-school.ro")).ValueOrThrow();
        var slogan = Slogan.Create("Professional driving education").ValueOrThrow();
        var description = Description.Create("A professional driving school dedicated to safe driving education.").ValueOrThrow();

        var phoneNumber = PhoneNumber.Create(_faker.Phone.PhoneNumber()).ValueOrThrow();
        var phoneNumberGroups = new List<PhoneNumbersGroup>
        {
            new PhoneNumbersGroup
            {
                Name = "Main Office",
                PhoneNumbers = [new PhoneNumber { Value = phoneNumber.Value }]
            }
        };

        var businessHours = CreateBusinessHours();
        var address = CreateRandomAddress();
        var city = cities.First();
        var statistics = new Statistics { NumberOfStudents=_faker.Random.Int(50, 200) };
        var selectedCategories = vehicleCategories.Take(1).ToList();
        var selectedCertificates = new List<ArrCertificate>();
        var categoryPricings = new List<SchoolCategoryPricing>();
        var team = Team.Create(Guid.NewGuid());

        var school = School.Create(
            owner,
            name,
            legalName,
            email,
            phoneNumber,
            phoneNumberGroups,
            businessHours,
            selectedCategories,
            selectedCertificates,
            null,
            city,
            slogan,
            description,
            address,
            statistics,
            team
        );

        school.SetCategoryPricings(categoryPricings);

        _context.Schools.Add(school);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created single school: {schoolName} with owner: {ownerEmail}", schoolName, ownerEmail);

        return school;
    }
}