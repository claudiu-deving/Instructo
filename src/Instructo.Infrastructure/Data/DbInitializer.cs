using System.Reflection;

using Domain.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Data;

public static class DbInitializer
{
    /// <summary>
    /// Applies pending migrations and seeds initial data
    /// </summary>
    public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider, ILogger logger)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        try
        {
            // Apply any pending migrations
            logger.LogInformation("Applying database migrations...");
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully.");

            // Seed roles and users
            await SeedRolesAndAdminUser(serviceProvider);

            // Seed schools if configured
            await SeedSchoolsIfConfiguredAsync(serviceProvider, logger);

            await CreateSchoolDetailsView(context);

            logger.LogInformation("Database seeding completed successfully.");
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }

    private static async Task CreateSchoolDetailsView(AppDbContext context)
    {
        await context.Database.ExecuteSqlRawAsync(@"CREATE VIEW SchoolDetails AS
SELECT         
s.Id, 
s.Name, 
s.CompanyName, 
s.Email, 
s.PhoneNumber, 
s.Slug, 
s.Slogan,
s.Description, 
county.Code AS CountyId,
city.Name AS CityName, 
s.PhoneNumbersGroups,
s.BussinessHours,
(SELECT 
        i.FileName,
        i.Url,
        i.Description,
        i.ContentType
        FROM dbo.Images AS i
        WHERE i.Id = s.IconId
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
) AS IconData,
(
    SELECT 
        link.Url,
        link.Name,
        link.Description,
        i.FileName as IconFileName,
        i.Url as IconUrl ,
        i.Description as IconDescription,
        i.ContentType as IconContentType
         FROM dbo.WebsiteLinks AS link
         JOIN dbo.Images i on i.Id = link.IconId
        WHERE link.SchoolId = s.Id
        FOR JSON PATH
) AS Links,
(SELECT 
        vc.Name,
        vc.Description
        FROM dbo.SchoolCategories AS sc
        JOIN dbo.VehicleCategories vc on sc.VehicleCategoryId = vc.Id
        WHERE sc.SchoolId= s.Id
        FOR JSON PATH
) AS VehicleCategories,
(SELECT 
        vc.Name,
        vc.Description
        FROM dbo.SchoolCertificates AS sc
        JOIN dbo.ARRCertificates vc on sc.CertificateId = vc.Id
        WHERE sc.SchoolId= s.Id
        FOR JSON PATH
) AS ArrCertificates,
(SELECT 
        vc.Name,
        scp.FullPrice,
        scp.InstallmentPrice,
        scp.Installments,
        t.Name Transmission,
        vc.Name as VehicleCategory
        FROM dbo.SchoolCategoryPricings AS scp
        JOIN dbo.VehicleCategories vc on scp.VehicleCategoryId = vc.Id
        JOIN dbo.Transmissions t on t.Id = scp.TransmissionId
        WHERE scp.SchoolId = s.Id
        FOR JSON PATH
) AS CategoryPricings,
(SELECT (
    SELECT
            i.LastName,
            i.FirstName,
            YEAR(GETDATE()) - i.BirthYear as Age,
            i.YearsExperience,
            i.Email,
            i.Gender,
            i.Specialization,
            i.Phone as PhoneNumber,
            images.FileName as ProfileImageName,
            images.Url as ProfileImageUrl,
            images.ContentType as ProfileImageContentType,
            images.Description as ProfileImageDescription,
              JSON_QUERY((
                SELECT vc.Name
                FROM dbo.InstructorVehicleCategories ivc
                JOIN dbo.VehicleCategories vc ON vc.Id = ivc.VehicleCategoriesId
                WHERE ivc.InstructorsId = i.Id
                FOR JSON PATH
            )) AS Categories
            FROM dbo.Teams AS t
            JOIN dbo.Instructors i on i.TeamId = t.Id
            JOIN dbo.Images as images on images.Id = i.ProfileImageId
            WHERE t.SchoolId = s.Id
            FOR JSON PATH) as Instructors
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
) AS Team,
(SELECT 
        CAST(ROUND(addr.Coordinate.Long, 6, 1) AS DECIMAL(10,6)) AS Longitude,
        CAST(ROUND(addr.Coordinate.Lat, 6, 1) AS DECIMAL(10,6)) AS Latitude,
        addr.Street,
        addr.Comment,
        addr.AddressType
        FROM dbo.Addresses AS addr
        WHERE addr.SchoolId = s.Id
        FOR JSON PATH
) AS ExtraLocations,
s.SchoolStatistics

FROM  dbo.Schools AS s  
JOIN dbo.Counties AS county ON s.CountyId = county.Id 
JOIN dbo.Cities AS city ON s.CityId = city.Id 
");
    }

    /// <summary>
    /// Alternative method for development/testing - ensures database is created
    /// </summary>
    public static async Task EnsureDatabaseCreatedAsync(IServiceProvider serviceProvider, ILogger logger)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        try
        {
            // Apply any pending migrations
            logger.LogInformation("Applying database migrations...");
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully.");

            // Seed roles and users if not already done
            await SeedRolesAndAdminUser(serviceProvider);

            // Check if we should use legacy SQL seeding or new SchoolSeeder
            var seedingOptions = scope.ServiceProvider.GetService<IOptions<SeedingOptions>>()?.Value??new SeedingOptions();


            await SeedSchoolsIfConfiguredAsync(serviceProvider, logger);
            await CreateSchoolDetailsView(context);

            logger.LogInformation("Database seeding completed successfully.");
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "An error occurred while ensuring database creation.");
            throw;
        }
    }

    private static async Task SeedSchoolsIfConfiguredAsync(IServiceProvider serviceProvider, ILogger logger)
    {
        using var scope = serviceProvider.CreateScope();
        var seedingOptions = scope.ServiceProvider.GetService<IOptions<SeedingOptions>>()?.Value??new SeedingOptions();

        if(!seedingOptions.EnableSchoolSeeding)
        {
            logger.LogInformation("School seeding is disabled in configuration.");
            return;
        }

        try
        {
            var schoolSeeder = SchoolSeeder.Create(serviceProvider);
            await schoolSeeder.SeedSchoolsAsync(seedingOptions.MinimumSchoolCount, seedingOptions.ForceReseed);
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding schools.");
            // Don't throw in development - log the error but continue
            var environment = scope.ServiceProvider.GetService<IHostEnvironment>();
            if(environment?.IsProduction()==true)
            {
                throw;
            }
        }
    }

    private static async Task SeedLegacyTestDataAsync(AppDbContext context, ILogger logger)
    {
        try
        {
            logger.LogInformation("Using legacy SQL test data seeding...");

            context.Database.BeginTransaction();
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Infrastructure.Data.Hardcoded.InsertTestSchools.sql";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if(stream==null)
            {
                logger.LogWarning("Legacy test data SQL file not found. Skipping legacy seeding.");
                return;
            }

            using var reader = new StreamReader(stream);
            var sqlContent = await reader.ReadToEndAsync();

            if(!string.IsNullOrWhiteSpace(sqlContent))
            {
                await context.Database.ExecuteSqlRawAsync(sqlContent);
                context.Database.CommitTransaction();
                logger.LogInformation("Legacy test data seeded successfully.");
            }
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Failed to seed legacy test data.");
            context.Database.RollbackTransaction();
            throw;
        }
    }

    public static async Task SeedRolesAndAdminUser(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        if(roleManager.Roles.Any())
            return;
        // Create roles
        string[] roleNames = ["Admin", "Owner", "Instructor", "Student"];

        await roleManager.CreateAsync(new ApplicationRole() { Name="IronMan", Description="I am Ironman" });
        await roleManager.CreateAsync(new ApplicationRole() { Name="Admin", Description="Can manage a school, cannot delete it" });
        await roleManager.CreateAsync(new ApplicationRole() { Name="Owner", Description="Can manage a school, can delete it" });
        await roleManager.CreateAsync(new ApplicationRole() { Name="Instructor", Description="Can manage session" });
        await roleManager.CreateAsync(new ApplicationRole() { Name="Student", Description="Can view session" });

        var ironMan = new ApplicationUser()
        {
            Id=Guid.Parse("8fb090d2-ad97-41d0-86ce-08ddc4a5a731"),
            Email="claudiu.c.strugar@gmail.com",
            LastName="Strugar",
            FirstName="Claudiu",
            PhoneNumber="1234567890",
            UserName="claudiu.c.strugar@gmail.com"
        };
        await userManager.CreateAsync(ironMan, "Password123!");
        await userManager.AddToRoleAsync(ironMan, "IronMan");
    }
}

