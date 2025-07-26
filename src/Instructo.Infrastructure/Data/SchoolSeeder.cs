using System.Threading.Tasks;

using Bogus;

using Domain.Dtos;
using Domain.Dtos.School;
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

public class SchoolSeeder(AppDbContext context, UserManager<ApplicationUser> userManager, ILogger<SchoolSeeder> logger)
{
    private readonly Faker _faker = new Faker("ro");
    private readonly GeometryFactory _geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(4326);

    // Romanian geographical boundaries
    private const double _minLatitude = 43.6;
    private const double _maxLatitude = 48.3;
    private const double _min_longitude = 20.3;
    private const double _max_Longitude = 29.7;

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
    private static readonly string[] _items = ["Car", "Motorcycle", "Truck"];
    private static readonly string[] _itemsArray = ["female", "male"];

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
            logger.LogInformation("Starting school seeding process for {count} schools", count);

            // Ensure we have required reference data
            await EnsureReferenceDataAsync();

            var existingSchoolsCount = await context.Schools.CountAsync();
            if(existingSchoolsCount>=count&&!includeTestData)
            {
                logger.LogInformation("Database already contains {existingCount} schools, skipping seeding", existingSchoolsCount);
                return;
            }

            // Create owner users first
            var owners = await CreateOwnerUsersAsync(count);

            // Get reference data
            var cities = await context.Cities.Include(c => c.County).ToListAsync();
            var vehicleCategories = await context.Categories.ToListAsync();
            var certificates = await context.CertificateTypes.ToListAsync();

            if(cities.Count==0)
            {
                logger.LogWarning("No cities found in database. Creating sample cities.");
                cities=await CreateSampleCitiesAsync();
            }

            if(vehicleCategories.Count==0)
            {
                logger.LogWarning("No vehicle categories found in database. This may cause issues.");
            }

            // Create schools
            var schools = new List<School>();
            for(int i = 0; i<count; i++)
            {
                try
                {
                    var school = await CreateSchoolAsync(owners[i], cities, vehicleCategories, certificates);
                    schools.Add(school);
                    logger.LogDebug("Created school: {schoolName}", school.Name);
                }
                catch(Exception ex)
                {
                    logger.LogError(ex, "Failed to create school {index}", i+1);
                }
            }



            // Save all schools
            if(schools.Count!=0)
            {
                context.Schools.AddRange(schools);
                await context.SaveChangesAsync();
                logger.LogInformation("Successfully seeded {count} schools", schools.Count);
            }
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Failed to seed schools");
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

            var result = await userManager.CreateAsync(user, "TempPassword123!");
            if(result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, ApplicationRole.User.Name!);
                owners.Add(user);
                logger.LogDebug("Created owner user: {email}", user.Email);
            }
            else
            {
                logger.LogWarning("Failed to create user {email}: {errors}",
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
        var cityName = _faker.PickRandom(context.Cities);
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

        var manualTransmission = context.Transmissions.FirstOrDefault(x => x.Id==1);
        // Create category pricings
        var categoryPricings = selectedCategories.Select(vc => new SchoolCategoryPricing
        {
            VehicleCategoryId=vc.Id,
            FullPrice=_faker.Random.Decimal(1000, 3000),
            Installments=_faker.Random.Int(1, 6),
            InstallmentPrice=_faker.Random.Decimal(200, 600),
            Transmission=manualTransmission

        }).ToList();


        // Create school icon (optional)
        Image? icon = CreateRandomImage();

        await userManager.AddToRoleAsync(owner, ApplicationRole.Owner.Name!);

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
            statistics
        );
        school.SetCategoryPricings(categoryPricings);

        for(int i = 0; i<_faker.Random.Int(1, 3); i++)
        {
            var extraLocation = CreateRandomAddress(AddressType.LessonStart);
            school.AddExtraLocation(extraLocation);
        }
        var main = CreateRandomAddress(AddressType.MainLocation);
        school.AddExtraLocation(main);

        for(int i = 0; i<_faker.Random.Int(1, 3); i++)
        {
            var randomLink = CreateRandomSocialMediaLink();
            school.AddLink(randomLink);
        }
        var team = school.CreateTeam(); // Will be updated with actual school ID after creation


        for(int i = 0; i<_faker.Random.Int(1, 30); i++)
        {
            var instructor = InstructorProfile.Create(
                _faker.Name.FirstName(),
                _faker.Name.LastName(),
                _faker.Random.Int(1980, 2000),
                _faker.Random.Int(1, 20),
                _faker.PickRandom(_items),
                _faker.Lorem.Sentence(10),
                _faker.Phone.PhoneNumber(),
                _faker.Internet.Email(),
                _faker.PickRandom(_itemsArray),
              CreateRandomImage(),
                [.. _faker.PickRandom(vehicleCategories, _faker.Random.Int(1, 3))]
                );
            team.AddInstructor(instructor);
        }

        return school;
    }

    private WebsiteLink CreateRandomSocialMediaLink()
    {
        var socialMediaPlatforms = new[] { "Facebook", "Instagram", "Twitter", "LinkedIn", "YouTube" };
        var platform = _faker.PickRandom(socialMediaPlatforms);
        var description = _faker.Lorem.Sentence(5);
        var url = _faker.Internet.UrlWithPath();
        var image = CreateRandomImage();

        return WebsiteLink.Create(platform, url, description, image!).ValueOrThrow();
    }

    private static BussinessHours CreateBusinessHours()
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

        var latitude = _faker.Random.Double(_minLatitude, _maxLatitude);
        var longitude = _faker.Random.Double(_min_longitude, _max_Longitude);

        return Address.Create(street, latitude.ToString("F6"), longitude.ToString("F6"), addressType).Value!;
    }



    private Image? CreateRandomImage()
    {
        var result = Image.Create(_faker.Image.PicsumUrl(width: 200, height: 200, false),
             "image/jpeg",
             _faker.Internet.UrlWithPath(),
             "School icon image")
             .OnSuccess(image => context.Images.Add(image))
             .OnError(errors => logger.LogWarning("Failed to create school icon: {errors}", string.Join(", ", errors.Select(e => e.Message))));


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

        context.Counties.AddRange(counties);

        // Create sample cities
        var cities = new List<City>
        {
            new City { Id = 1, Name = "București", County = counties[0] },
            new City { Id = 2, Name = "Cluj-Napoca", County = counties[1] },
            new City { Id = 3, Name = "Timișoara", County = counties[2] }
        };

        context.Cities.AddRange(cities);
        await context.SaveChangesAsync();

        return cities;
    }

    private async Task EnsureReferenceDataAsync()
    {
        // Check if we have required reference data, if not log warnings
        var citiesCount = await context.Cities.CountAsync();
        var vehicleCategoriesCount = await context.Categories.CountAsync();

        if(citiesCount==0)
        {
            logger.LogWarning("No cities found in database. Sample cities will be created.");
        }

        if(vehicleCategoriesCount==0)
        {
            logger.LogWarning("No vehicle categories found in database. Schools will be created without categories.");
        }
    }


    public async Task<School> SeedSchoolWithExplicitDataAsync(UpdateSchoolCommandDto explicitData, ApplicationUser? owner = null)
    {
        try
        {
            logger.LogInformation("Creating school with explicit data");

            // Create owner if not provided
            if(owner==null)
            {
                // Create a new unique user for each test to avoid conflicts
                var uniqueEmail = $"test-user-{Guid.NewGuid():N}@test.com";
                owner=new ApplicationUser
                {
                    Id=Guid.NewGuid(),
                    FirstName=_faker.Name.FirstName(),
                    LastName=_faker.Name.LastName(),
                    Email=uniqueEmail,
                    UserName=uniqueEmail,
                    NormalizedEmail=uniqueEmail.ToUpperInvariant(),
                    NormalizedUserName=uniqueEmail.ToUpperInvariant(),
                    EmailConfirmed=true,
                    IsActive=true,
                    Created=_faker.Date.PastOffset(2).DateTime,
                    PhoneNumber=_faker.Phone.PhoneNumber()
                };

                var result = await userManager.CreateAsync(owner, "TempPassword123!");
                if(!result.Succeeded)
                {
                    throw new InvalidOperationException($"Failed to create test user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }

                await userManager.AddToRoleAsync(owner, ApplicationRole.User.Name!);
                logger.LogDebug("Created new test user: {email}", owner.Email);
            }

            // Get reference data
            var cities = await context.Cities.Include(c => c.County).ToListAsync();
            var vehicleCategories = await context.Categories.ToListAsync();
            var certificates = await context.CertificateTypes.ToListAsync();

            // Ensure we have at least some reference data for tests
            if(cities.Count==0)
            {
                cities=await CreateSampleCitiesAsync();
            }

            if(vehicleCategories.Count==0)
            {
                logger.LogWarning("No vehicle categories found in database. This may cause issues.");
                // You might want to seed some basic vehicle categories here for tests
            }

            // Create base school with random data
            var baseSchool = await CreateSchoolAsync(owner, cities, vehicleCategories, certificates);

            // Override with explicit values
            if(explicitData.Name!=null)
            {
                var name = SchoolName.Create(explicitData.Name).ValueOrThrow();
                baseSchool.UpdateName(name);
            }

            if(explicitData.LegalName!=null)
            {
                var legalName = LegalName.Create(explicitData.LegalName).ValueOrThrow();
                baseSchool.UpdateLegalName(legalName);
            }

            if(explicitData.SchoolEmail!=null)
            {
                var email = Email.Create(explicitData.SchoolEmail).ValueOrThrow();
                baseSchool.UpdateEmail(email);
            }

            if(explicitData.City!=null)
            {
                var city = cities.FirstOrDefault(c => c.Name==explicitData.City)
                    ??throw new InvalidOperationException($"City {explicitData.City} not found");
                baseSchool.UpdateCity(city);
            }

            if(explicitData.Slogan!=null)
            {
                var slogan = Slogan.Create(explicitData.Slogan).ValueOrThrow();
                baseSchool.UpdateSlogan(slogan);
            }

            if(explicitData.Description!=null)
            {
                var description = Description.Create(explicitData.Description).ValueOrThrow();
                baseSchool.UpdateDescription(description);
            }

            if(explicitData.PhoneNumber!=null)
            {
                var phoneNumber = PhoneNumber.Create(explicitData.PhoneNumber).ValueOrThrow();
                baseSchool.UpdatePhoneNumber(phoneNumber);
            }

            // Handle main location
            if(explicitData.MainLocationStreet!=null||
                explicitData.MainLocationLatitude!=null||
                explicitData.MainLocationLongitude!=null)
            {
                var street = explicitData.MainLocationStreet??_faker.Address.StreetAddress();
                var lat = explicitData.MainLocationLatitude??_faker.Random.Double(_minLatitude, _maxLatitude).ToString("F6");
                var lng = explicitData.MainLocationLongitude??_faker.Random.Double(_min_longitude, _max_Longitude).ToString("F6");

                var mainLocation = Address.Create(street, lat, lng, AddressType.MainLocation).ValueOrThrow();
                baseSchool.UpdateMainLocation(mainLocation);
            }

            // Handle collections
            if(explicitData.VehiclesCategories!=null)
            {
                var categories = vehicleCategories
                    .Where(vc => explicitData.VehiclesCategories.Contains(vc.Name))
                    .ToList();
                baseSchool.UpdateVehicleCategories(categories);
            }

            if(explicitData.CategoryPricings!=null)
            {
                var pricings = explicitData.CategoryPricings.Select(cp => new SchoolCategoryPricing
                {
                    VehicleCategoryId=vehicleCategories.First(vc => vc.Name==cp.VehicleCategory).Id,
                    FullPrice=cp.FullPrice,
                    Installments=cp.Installments,
                    InstallmentPrice=cp.InstallmentPrice,
                    Transmission=context.Transmissions.FirstOrDefault(t => t.Name==cp.Transmission)
                }).ToList();

                baseSchool.SetCategoryPricings(pricings);
            }

            if(explicitData.NumberOfStudents.HasValue)
            {
                baseSchool.SchoolStatistics.NumberOfStudents=explicitData.NumberOfStudents.Value;
            }

            // Save the school
            context.Schools.Add(baseSchool);
            await context.SaveChangesAsync();

            logger.LogInformation("Successfully created school with explicit data: {schoolName}", baseSchool.Name);
            return baseSchool;
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Failed to create school with explicit data");
            throw;
        }
    }
}