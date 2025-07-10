using Application.Schools.Commands.CreateSchool;
using Application.Users.Commands.RegisterUser;

using Domain.Entities;
using Domain.Entities.SchoolEntities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Shared;
using Domain.ValueObjects;

using JetBrains.Annotations;

using Messager;

using Moq;

namespace Instructo.UnitTests.Application.Schools.Commands.CreateSchools;

public class CreateSchoolCommandTests
{
    private readonly CreateSchoolCommandHandler _handler;
    private readonly Mock<ISchoolManagementDirectory> _schoolManagementDirectoryMock;
    private readonly Mock<ISocialMediaPlatformImageProvider> _socialMediaProviderMock;
    private readonly Mock<Microsoft.AspNetCore.Identity.RoleManager<ApplicationRole>> _roleManagerMock;
    private readonly Mock<ISender> _senderMock;
    private readonly Mock<IIdentityService> _identityServiceMock;

    public CreateSchoolCommandTests()
    {
        _schoolManagementDirectoryMock=new Mock<ISchoolManagementDirectory>();
        _socialMediaProviderMock=new Mock<ISocialMediaPlatformImageProvider>();
        _roleManagerMock=new Mock<Microsoft.AspNetCore.Identity.RoleManager<ApplicationRole>>(
            Mock.Of<Microsoft.AspNetCore.Identity.IRoleStore<ApplicationRole>>(), null, null, null, null);
        _senderMock=new Mock<ISender>();
        _identityServiceMock=new Mock<IIdentityService>();

        _handler=new CreateSchoolCommandHandler(
            _schoolManagementDirectoryMock.Object,
            _socialMediaProviderMock.Object,
            _roleManagerMock.Object,
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
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Code==userRegistrationError.Code);
        _schoolManagementDirectoryMock.Verify(r => r.SchoolCommandRepository.AddAsync(It.IsAny<School>()), Times.Never);
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
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Code=="Create-School");
        _schoolManagementDirectoryMock.Verify(r => r.SchoolCommandRepository.AddAsync(It.IsAny<School>()), Times.Never);
    }
}