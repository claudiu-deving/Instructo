using Instructo.Application.Abstractions.Messaging;
using Instructo.Domain.Interfaces;
using Instructo.Domain.Shared;

namespace Instructo.Application.Users.Commands.RegisterUser;

public class RegisterUserCommandHandler(IIdentityService identityService) : ICommandHandler<RegisterUserCommand, Result<string>>
{
    public async Task<Result<string>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {

        var userRegistrationRequest = await identityService.RegisterAsync(new Domain.Dtos.RegisterUserDto()
        {
            Email=request.Email,
            FirstName=request.FirstName,
            LastName=request.LastName,
            Password=request.Password
        });
        return userRegistrationRequest;
    }
}
