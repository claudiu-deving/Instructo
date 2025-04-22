using Application.Schools.Commands.CreateSchool;
using Application.Users.Commands.RegisterUser;

using Domain.Entities;
using Domain.Entities.SchoolEntities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Shared;
using Domain.ValueObjects;

using FluentAssertions;

using MediatR;

using Microsoft.Extensions.Logging;

using Moq;

namespace Instructo.UnitTests.Application.Schools.Commands.CreateSchools;


public class CreateSchoolCommandTests
{
    private readonly Mock<ICommandRepository<School, SchoolId>> _repositoryMock;
    private readonly Mock<IQueryRepository<School, SchoolId>> _queryRepositoryMock;
    private readonly Mock<IQueryRepository<ArrCertificate, ARRCertificateType>> _certificatesRepositoryMock;
    private readonly Mock<IQueryRepository<VehicleCategory, VehicleCategoryType>> _vehicleCategoriesRepositoryMock;
    private readonly Mock<ILogger<CreateSchoolCommandHandler>> _loggerMock;
    private readonly Mock<ISocialMediaPlatformImageProvider> _socialMediaProviderMock;
    private readonly Mock<IIdentityService> _identityServiceMock;
    private readonly Mock<ISender> _senderMock;
    private readonly CreateSchoolCommandHandler _handler;

    public CreateSchoolCommandTests()
    {
        _repositoryMock=new Mock<ICommandRepository<School, SchoolId>>();
        _queryRepositoryMock=new Mock<IQueryRepository<School, SchoolId>>();
        _certificatesRepositoryMock=new Mock<IQueryRepository<ArrCertificate, ARRCertificateType>>();
        _vehicleCategoriesRepositoryMock=new Mock<IQueryRepository<VehicleCategory, VehicleCategoryType>>();
        _loggerMock=new Mock<ILogger<CreateSchoolCommandHandler>>();
        _socialMediaProviderMock=new Mock<ISocialMediaPlatformImageProvider>();
        _identityServiceMock=new Mock<IIdentityService>();
        _senderMock=new Mock<ISender>();
        _vehicleCategoriesRepositoryMock.Setup(x => x.GetByIdAsync(VehicleCategoryType.A1)).ReturnsAsync(VehicleCategory.Create(VehicleCategoryType.A1, "Test"));
        _handler=new CreateSchoolCommandHandler(
            _repositoryMock.Object,
            _queryRepositoryMock.Object,
            _certificatesRepositoryMock.Object,
            _vehicleCategoriesRepositoryMock.Object,
            _socialMediaProviderMock.Object,
            _senderMock.Object);
    }


    [Fact]
    public async Task Handle_WhenUserRegistrationFails_ReturnsFailureResult()
    {
        // Arrange
        var command = TestCommandFactory.CreateValidSchoolCommand();
        var userRegistrationError = new Error("User-Registration", "Failed to register user");

        _senderMock.Setup(s => s.Send(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ApplicationUser>.Failure([userRegistrationError]));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code==userRegistrationError.Code);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<School>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserRetrievalFails_ReturnsFailureResult()
    {
        // Arrange
        var command = TestCommandFactory.CreateValidSchoolCommand();

        _senderMock.Setup(s => s.Send(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ApplicationUser>.Failure(new Error("Create-School", "Error")));
        _identityServiceMock.Setup(i => i.GetUserByEmailAsync(command.OwnerEmail))
            .ReturnsAsync((ApplicationUser)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code=="Create-School");
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<School>()), Times.Never);
    }

    private void SetupSuccessfulDependencies(
       CreateSchoolCommand command,
       ApplicationUser user,
       School school)
    {
        // Setup successful user registration
        _senderMock.Setup(s => s.Send(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ApplicationUser>.Success(user));

        _identityServiceMock.Setup(i => i.GetUserByEmailAsync(command.OwnerEmail))
            .ReturnsAsync(user);

        // Setup successful social media platform provider
        _socialMediaProviderMock.Setup(p => p.Get(It.IsAny<string>()))
            .Returns(new SocialMediatPlatform(
                  "/path/to/icon",
               "image/png",
                SocialMediaPlatforms.Facebook,
               "Platform description"
            ));

        // Setup successful repository add
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<School>()))
            .ReturnsAsync(Result<School>.Success(school));

        _queryRepositoryMock.Setup(queryRepo =>
        queryRepo.GetByIndexed(command.LegalName))
            .ReturnsAsync(Result<IEnumerable<School>?>.Success([]));

    }
}
