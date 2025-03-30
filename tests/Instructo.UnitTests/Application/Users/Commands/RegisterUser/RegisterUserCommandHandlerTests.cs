using Instructo.UnitTests.Application.Users.Common;
using Instructo.Application.Users.Commands.RegisterUser;
using Instructo.Domain.Interfaces;
using Moq;
using System.Threading.Tasks;
using Instructo.Domain.Dtos;
using Instructo.Domain.Shared;
using MediatR;
namespace Instructo.UnitTests.Application.Users.Commands.RegisterUser;

public class RegisterUserCommandHandlerTests : BaseTest
{
    private readonly RegisterUserCommandHandler _sut;

    public RegisterUserCommandHandlerTests()
    {
        var mockedIdentityService = new Mock<IIdentityService>();
        mockedIdentityService.Setup(mockedIdentityService => mockedIdentityService.RegisterAsync(It.IsAny<RegisterUserDto>())).ReturnsAsync(Result<string>.Success("token"));
        _sut=new RegisterUserCommandHandler(mockedIdentityService.Object);
    }

    [Fact]
    public async Task RegisterRandomUser_ReturnsToken()
    {
        var randomUser = UserBuilder.Build();
        var registerUserCommand = new RegisterUserCommand(randomUser.Email, randomUser.FirstName, randomUser.LastName, randomUser.PasswordHash, randomUser.PhoneNumber);

        var response = await _sut.Handle(registerUserCommand, CancellationToken.None);
        Assert.NotNull(response);
        response.Match(ok =>
        {
            Assert.Equal("token", ok);
            return Unit.Value;
        }, err =>
        {
            Assert.Fail("Should not return error");
            return Unit.Value;
        });
    }
}
