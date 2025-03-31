using Instructo.Application.Users.Commands.RegisterUser;
using Instructo.Domain.Entities;
using Instructo.Domain.Interfaces;
using Instructo.Domain.Shared;
using Instructo.Infrastructure.Identity;
using Instructo.UnitTests.Application.Users.Common;

using MediatR;

using Microsoft.AspNetCore.Identity;

using Moq;
namespace Instructo.UnitTests.Application.Users.Commands.RegisterUser;

public class RegisterUserCommandHandlerTests : BaseTest
{
    private readonly Mock<IIdentityService> _mockedIdentityService;
    private RegisterUserCommandHandler _sut;

    public RegisterUserCommandHandlerTests()
    {
        var userManager = new UserManager<ApplicationUser>()
        _mockedIdentityService=new IdentityService()
    }

    [Fact]
    public async Task RegisterRandomUser_ReturnsToken()
    {

    }
}
