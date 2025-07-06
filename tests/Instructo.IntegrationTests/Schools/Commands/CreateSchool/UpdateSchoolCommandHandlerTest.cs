using Application.Abstractions.Messaging;
using Application.Schools.Commands.CreateSchool;
using Application.Schools.Commands.UpdateSchool;
using Application.Users.Commands.RegisterUser;
using Application.Users.Queries.GetUserById;

using Domain.Dtos.Link;
using Domain.Dtos.School;
using Domain.Entities;
using Domain.Entities.SchoolEntities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Shared;
using Domain.ValueObjects;

using FluentAssertions;

using JetBrains.Annotations;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

using Moq;

namespace Instructo.IntegrationTests.Schools.Commands.CreateSchool;

[TestSubject(typeof(CreateSchoolCommandHandler))]
public class CreateSchoolCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreateSchoolName_WhenCommandIsValid()
    {
        // Arrange
        var serviceProvider = ConfigureServices();
        var handler = serviceProvider.GetRequiredService<IRequestHandler<CreateSchoolCommand, Result<SchoolReadDto>>>();
        var requestingUser = CreateTestUser();
        var existingSchool = CreateTestSchool(requestingUser);

        //  var mockCreateUserCommandHandler = new Mock<ICommandHandler<RegisterUserCommand, Result<ApplicationUser>>>();
        //   mockCreateUserCommandHandler.Setup(x => x.Handle(It.IsAny<RegisterUserCommand>(), CancellationToken.None)).ReturnsAsync(requestingUser);

        var mockVehiclesCategoriesRepository = serviceProvider.GetRequiredService<Mock<IQueryRepository<VehicleCategory, VehicleCategoryType>>>()
            .Setup(x => x.GetByIdAsync(VehicleCategoryType.B));

        var mockSchoolQueryRepository = serviceProvider.GetRequiredService<Mock<IQueryRepository<School, SchoolId>>>();
        mockSchoolQueryRepository.Setup(x => x.GetByIdAsync(It.IsAny<SchoolId>()))
            .ReturnsAsync(Result<School?>.Success(existingSchool));
        var mockMediator = serviceProvider.GetRequiredService<Mock<ISender>>();

        mockMediator.Setup(x => x.Send(It.IsAny<GetUserByIdQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ApplicationUser>.Success(requestingUser));
        mockMediator.Setup(x => x.Send(It.IsAny<RegisterUserCommand>(), CancellationToken.None)).ReturnsAsync(requestingUser);
        var mockSchoolCommandRepository =
            serviceProvider.GetRequiredService<Mock<ISchoolCommandRepository>>();
        mockSchoolCommandRepository.Setup(x => x.AddAsync(It.IsAny<School>()))
            .ReturnsAsync(existingSchool);
        mockSchoolCommandRepository.Setup(x => x.UpdateAsync(It.IsAny<School>()))
            .ReturnsAsync(Result<School>.Success(existingSchool));

        var newSchoolName = SchoolName.Wrap("Updated School Name");
        var commandDto = new CreateSchoolCommandDto(
            "Test",
            "Test Company SRL",
            "Owner@email.com",
            "contact@schoo.ro",
            "somePass123!",
            "John",
            "Doe",
            "London",
            "123 Street",
            "0758455151",
            "src/image",
            "png.image",
            [],
            WebsiteLink.Create("url.com", "Some name", "description", Image.Create("name", "png/image", "some-url", "desc").Value!).Value!.ToDto(),
            [],
            [],
            ["B"],
            []);
        var command = CreateSchoolCommand.Create(commandDto);

        // Act
        var result = await handler.Handle(command.Value!, CancellationToken.None);

        // Assert
        if(result.IsError)
        {
            result.IsError.Should()
                .BeFalse($"{string.Join(Environment.NewLine, result.Errors.ToList())}");
        }
        else
        {
            result.Value.Should().NotBeNull();
        }
        mockSchoolCommandRepository.Verify(x
            => x.UpdateAsync(It.Is<School>(s => s.Name==newSchoolName)), Times.Once);
    }

    private static School CreateTestSchool(ApplicationUser requestingUser)
    {
        var school = new School(
            requestingUser,
            SchoolName.Wrap("Test School"),
            LegalName.Wrap("Test School LLC"),
            Email.Create("school@example.com").Value!,
            PhoneNumber.Create("555-123-4567").Value!,
            [],
            BussinessHours.Empty,
            [],
            [],
            null
        );

        var websiteImage = Image.Create(
            "Test Image",
            ContentType.Create("image/png").Value!,
            FilePath.Create("/path/to/image.png").Value!,
            "Test Description"
        ).Value!;

        var websiteLink = WebsiteLink.Create(
            Url.Create("https://example.com").Value!,
            WebsiteLinkName.Wrap("Test Website"),
            "Company Website Icon",
            websiteImage
        ).Value!;

        school.AddLink(websiteLink);

        var schoolLogo = Image.Create(
            "Test Logo",
            ContentType.Create("image/png").Value!,
            FilePath.Create("/path/to/logo.png").Value!,
            "Test Logo Description"
        );
        school.AddLogo(schoolLogo.Value!);


        return school;
    }

    private static ApplicationUser CreateTestUser()
    {
        return new ApplicationUser
        {
            Id=Guid.NewGuid(),
            Email="user@example.com",
            FirstName="John",
            LastName="Doe"
        };
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Mock repositories
        var mockSchoolQueryRepository = new Mock<IQueryRepository<School, SchoolId>>();
        var mockVehicleCategoryRepository = new Mock<IQueryRepository<VehicleCategory, VehicleCategoryType>>();
        var mockCertificateRepository = new Mock<IQueryRepository<ArrCertificate, ARRCertificateType>>();
        var mockSchoolCommandRepository = new Mock<ISchoolCommandRepository>();
        var mockSocialMediaProvider = new Mock<ISocialMediaPlatformImageProvider>();
        var mockMediator = new Mock<ISender>();
        services.AddScoped(_ => mockSchoolQueryRepository);
        services.AddScoped(_ => mockSchoolQueryRepository.Object);
        services.AddScoped(_ => mockVehicleCategoryRepository);
        services.AddScoped(_ => mockVehicleCategoryRepository.Object);
        services.AddScoped(_ => mockCertificateRepository);
        services.AddScoped(_ => mockCertificateRepository.Object);
        services.AddScoped(_ => mockSchoolCommandRepository);
        services.AddScoped(_ => mockSchoolCommandRepository.Object);
        services.AddScoped(_ => mockSocialMediaProvider);
        services.AddScoped(_ => mockSocialMediaProvider.Object);
        services.AddScoped(_ => mockMediator);
        services.AddScoped(_ => mockMediator.Object);

        services.AddScoped<IRequestHandler<CreateSchoolCommand, Result<SchoolReadDto>>, CreateSchoolCommandHandler>();

        return services.BuildServiceProvider();
    }
}