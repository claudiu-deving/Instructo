using Instructo.Application.Abstractions.Messaging;
using Instructo.Domain.Interfaces;
using Instructo.Domain.Shared;

using Microsoft.Extensions.Logging;

namespace Instructo.Application.Users.Commands.ChangePassword;

public class ChangePasswordCommandHandler(IIdentityService identityService, ILogger<ChangePasswordCommandHandler> logger) : ICommandHandler<ChangePasswordCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await identityService.GetUserByEmailAsync(request.Email);
        if(user is null)
        {
            logger.LogError("Change password request for non-existing user: {Email}", request.Email);
            return Result<string>.Failure([new Error("Change-Password", "User doesn't exist")]);
        }
        return await identityService.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
    }
}