using Application.Schools.Commands.CreateSchool;
using Application.Users.Commands.RegisterUser;
using Domain.Entities;
using Domain.Entities.SchoolEntities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using JetBrains.Annotations;
using MediatR;
using Moq;

namespace Instructo.UnitTests.Application.Schools.Commands.CreateSchools;

[TestSubject(typeof(CreateSchoolCommandHandler))]
public class CreateSchoolCommandTests
{
    private readonly CreateSchoolCommandHandler _handler;
    private readonly Mock<IIdentityService> _identityServiceMock;
    private readonly Mock<ISchoolCommandRepository> _repositoryMock;
    private readonly Mock<ISender> _senderMock;

    public CreateSchoolCommandTests()
    {
        _repositoryMock = new Mock<ISchoolCommandRepository>();
        var queryRepositoryMock = new Mock<IQueryRepository<School, SchoolId>>();
        var certificatesRepositoryMock = new Mock<IQueryRepository<ArrCertificate, ARRCertificateType>>();
        var vehicleCategoriesRepositoryMock = new Mock<IQueryRepository<VehicleCategory, VehicleCategoryType>>();
        var socialMediaProviderMock = new Mock<ISocialMediaPlatformImageProvider>();
        _identityServiceMock = new Mock<IIdentityService>();
        _senderMock = new Mock<ISender>();
        vehicleCategoriesRepositoryMock.Setup(x
            => x.GetByIdAsync(VehicleCategoryType.A1)).ReturnsAsync(
            VehicleCategory.Create(VehicleCategoryType.A1, "Test"));

        _handler = new CreateSchoolCommandHandler(
            _repositoryMock.Object,
            queryRepositoryMock.Object,
            certificatesRepositoryMock.Object,
            vehicleCategoriesRepositoryMock.Object,
            socialMediaProviderMock.Object,
            _senderMock.Object);
    }


    [Fact]
    public async Task Handle_WhenUserRegistrationFails_ReturnsFailureResult()
    {
        // Arrange
        var command = TestCommandFactory.CreateValidSchoolCommand();
        var userRegistrationError = new Error("User-Registration", "Failed to register user");

        _senderMock.Setup(s
                => s.Send(It.IsAny<RegisterUserCommand>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ApplicationUser>.Failure(userRegistrationError));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == userRegistrationError.Code);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<School>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserRetrievalFails_ReturnsFailureResult()
    {
        // Arrange
        var command = TestCommandFactory.CreateValidSchoolCommand();

        _senderMock.Setup(s
                => s.Send(It.IsAny<RegisterUserCommand>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ApplicationUser>.Failure(new Error("Create-School", "Error")));

        _identityServiceMock.Setup(i => i.GetUserByEmailAsync(command.OwnerEmail))
            .ReturnsAsync((ApplicationUser?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "Create-School");
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<School>()), Times.Never);
    }
}