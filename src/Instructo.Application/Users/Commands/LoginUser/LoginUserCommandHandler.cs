using Application.Abstractions.Messaging;

using Domain.Interfaces;
using Domain.Shared;

using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.LoginUser;

public class LoginUserCommandHandler(IIdentityService identityService, ILogger<LoginUserCommandHandler> logger) : ICommandHandler<LoginUserCommand, Result<string>>
{
    public async Task<Result<string>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await identityService.GetUserByEmailAsync(request.Email);
        if(user is null)
            return Result<string>.Failure([new Error("Login-User", "User not found")]);
        var loginRequest = await identityService.CheckPasswordSignInAsync(user, request.Password);
        if(loginRequest.IsError)
        {
            user.AccessFailedCount++;
            var updateAttemptsCountRequest = await identityService.UpdateAsync(user);
            if(updateAttemptsCountRequest.IsError)
                logger.LogError("Failed to update user access failed count: {Email}", request.Email);
            return loginRequest;
        }
        else
        {
            return Result<string>.Success(await identityService.GenerateJwtTokenAsync(user));
        }
    }
}
