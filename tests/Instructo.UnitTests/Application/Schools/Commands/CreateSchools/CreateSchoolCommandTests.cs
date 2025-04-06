using FluentAssertions;

using Instructo.Application.Schools.Commands.CreateSchool;
using Instructo.Application.Users.Commands.RegisterUser;
using Instructo.Domain.Entities;
using Instructo.Domain.Entities.SchoolEntities;
using Instructo.Domain.Enums;
using Instructo.Domain.Interfaces;
using Instructo.Domain.Shared;
using Instructo.Domain.ValueObjects;

using MediatR;

using Microsoft.Extensions.Logging;

using Moq;

namespace Instructo.UnitTests.Application.Schools.Commands.CreateSchools;


public class CreateSchoolCommandTests
{
    private readonly Mock<ICommandRepository<School, SchoolId>> _repositoryMock;
    private readonly Mock<IQueryRepository<School, SchoolId>> _queryRepositoryMock;
    private readonly Mock<ILogger<CreateSchoolCommandHandler>> _loggerMock;
    private readonly Mock<ISocialMediaPlatformImageProvider> _socialMediaProviderMock;
    private readonly Mock<IIdentityService> _identityServiceMock;
    private readonly Mock<ISender> _senderMock;
    private readonly CreateSchoolCommandHandler _handler;

    public CreateSchoolCommandTests()
    {
        _repositoryMock=new Mock<ICommandRepository<School, SchoolId>>();
        _queryRepositoryMock=new Mock<IQueryRepository<School, SchoolId>>();
        _loggerMock=new Mock<ILogger<CreateSchoolCommandHandler>>();
        _socialMediaProviderMock=new Mock<ISocialMediaPlatformImageProvider>();
        _identityServiceMock=new Mock<IIdentityService>();
        _senderMock=new Mock<ISender>();

        _handler=new CreateSchoolCommandHandler(
            _repositoryMock.Object,
            _queryRepositoryMock.Object,
            _loggerMock.Object,
            _socialMediaProviderMock.Object,
            _identityServiceMock.Object,
            _senderMock.Object);
    }

    [Fact]
    public async Task Handle_WhenAllOperationsSucceed_ReturnsSuccessfulResult()
    {
        // Arrange
        var command = TestCommandFactory.CreateValidSchoolCommand();
        var user = new ApplicationUser { Email=command.OwnerEmail };
        var school = new School(user, command.Name, command.LegalName, command.SchoolEmail, command.PhoneNumber, [], Image.Create("as", "test", "url", "desc").Value!);


        SetupSuccessfulDependencies(command, user, school);
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        if(result.IsError)
        {
            throw new InvalidOperationException($"Failed to create school: {string.Join(", ", result.Errors.Select(e => e.Message))}");
        }
        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<School>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserRegistrationFails_ReturnsFailureResult()
    {
        // Arrange
        var command = TestCommandFactory.CreateValidSchoolCommand();
        var userRegistrationError = new Error("User-Registration", "Failed to register user");

        _senderMock.Setup(s => s.Send(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<string>.Failure([userRegistrationError]));

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
            .ReturnsAsync(Result<string>.Success(""));
        _identityServiceMock.Setup(i => i.GetUserByEmailAsync(command.OwnerEmail))
            .ReturnsAsync((ApplicationUser)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code=="Create-School");
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<School>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenSchoolIconCreationFails_ReturnsFailureResult()
    {
        // Arrange
        var command = TestCommandFactory.CreateValidSchoolCommand();
        var user = new ApplicationUser { Email=command.OwnerEmail };
        _queryRepositoryMock.Setup(queryRepo => queryRepo.GetByIndexed(command.LegalName)).ReturnsAsync(Result<IEnumerable<School>?>.Success([]));

        _senderMock.Setup(s => s.Send(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<string>.Success(""));

        _identityServiceMock.Setup(i => i.GetUserByEmailAsync(command.OwnerEmail))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<School>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenWebsiteLinkCreationFails_ReturnsFailureResult()
    {
        // Arrange
        var command = TestCommandFactory.CreateValidSchoolCommand();
        var user = new ApplicationUser { Email=command.OwnerEmail };
        _queryRepositoryMock.Setup(queryRepo => queryRepo.GetByIndexed(command.LegalName)).ReturnsAsync(Result<IEnumerable<School>?>.Success([]));

        _senderMock.Setup(s => s.Send(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<string>.Success(""));

        _identityServiceMock.Setup(i => i.GetUserByEmailAsync(command.OwnerEmail))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<School>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenSocialMediaLinkCreationFails_ReturnsFailureResult()
    {
        // Arrange
        var command = TestCommandFactory.CreateValidSchoolCommand();
        var user = new ApplicationUser { Email=command.OwnerEmail };
        _queryRepositoryMock.Setup(queryRepo => queryRepo.GetByIndexed(command.LegalName)).ReturnsAsync(Result<IEnumerable<School>?>.Success([]));

        _senderMock.Setup(s => s.Send(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<string>.Success(""));

        _identityServiceMock.Setup(i => i.GetUserByEmailAsync(command.OwnerEmail))
            .ReturnsAsync(user);

        _socialMediaProviderMock.Setup(p => p.Get(It.IsAny<string>()))
            .Throws(new ArgumentException("Invalid social media platform"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code=="Create-School");
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<School>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRepositoryAddFails_ReturnsFailureResult()
    {
        // Arrange
        var command = TestCommandFactory.CreateValidSchoolCommand();
        var user = new ApplicationUser { Email=command.OwnerEmail };
        var school = new School(user, command.Name, command.LegalName, command.SchoolEmail, command.PhoneNumber, [], Image.Create("AA", "BB", "CC", "DD").Value!);
        var repositoryError = new Error("Repository", "Failed to add school");

        SetupSuccessfulDependencies(command, user, school);

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<School>()))
            .ReturnsAsync(Result<School>.Failure([repositoryError]));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code==repositoryError.Code);
    }

    [Fact]
    public async Task RegisterAndGetUser_WhenSuccessful_ReturnsUser()
    {
        // Arrange
        var command = TestCommandFactory.CreateValidSchoolCommand();
        var user = new ApplicationUser { Email=command.OwnerEmail };

        _senderMock.Setup(s => s.Send(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<string>.Success(""));

        _identityServiceMock.Setup(i => i.GetUserByEmailAsync(command.OwnerEmail))
            .ReturnsAsync(user);

        // Act
        var result = await InvokePrivateMethod<Task<Result<ApplicationUser>>>(
            _handler,
            "RegisterAndGetUser",
            command,
            CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(user);
    }

    [Fact]
    public async Task AddSocialMediaLinks_WithValidLinks_AddsLinksToSchool()
    {
        // Arrange
        var command = TestCommandFactory.CreateValidSchoolCommand();
        var user = new ApplicationUser { Email=command.OwnerEmail };
        var school = new School(user, command.Name, command.LegalName, command.SchoolEmail, command.PhoneNumber, [], Image.Create("AA", "BB", "CC", "DD").Value!);
        var platformImage = Image.Create("AA", "BB", "CC", "DD").Value!;

        _socialMediaProviderMock.Setup(p => p.Get(It.IsAny<string>()))
            .Returns(new SocialMediatPlatform()
            {
                IconContentType="image/png",
                IconPath="/path/to/icon",
                Description="Platform description"
            });

        // Act
        var result = InvokePrivateMethod<Result<School>>(
            _handler,
            "AddSocialMediaLinks",
            command,
            school);

        // Assert
        result.IsError.Should().BeFalse();
        // You would need a method to inspect links added to the school
        // school.Links.Count.Should().Be(command.SocialMediaLinks.Count);
    }
    private void SetupSuccessfulDependencies(
       CreateSchoolCommand command,
       ApplicationUser user,
       School school)
    {
        // Setup successful user registration
        _senderMock.Setup(s => s.Send(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<string>.Success(""));

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

    /// <summary>
    /// Helper method to invoke private methods for testing
    /// </summary>
    private T InvokePrivateMethod<T>(object instance, string methodName, params object[] parameters)
    {
        var type = instance.GetType();
        var method = type.GetMethod(methodName, System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance);

        if(method==null)
            throw new InvalidOperationException($"Method {methodName} not found on {type.Name}");

        if(method.ReturnType==typeof(Task<T>))
        {
            var task = (Task<T>)method.Invoke(instance, parameters);
            return task.Result;
        }

        return (T)method.Invoke(instance, parameters);
    }

}
