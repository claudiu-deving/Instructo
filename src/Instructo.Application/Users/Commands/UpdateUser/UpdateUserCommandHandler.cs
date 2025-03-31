using Instructo.Application.Abstractions.Messaging;
using Instructo.Domain.Interfaces;
using Instructo.Domain.Shared;

using Microsoft.Extensions.Logging;

namespace Instructo.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler(IIdentityService identityService, ILogger<UpdateUserCommandHandler> logger) : ICommandHandler<UpdateUserCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var currentUser = await identityService.GetUserByIdAsync(request.Id);
        if(currentUser is null)
        {
            logger.LogError("Update user request for non-existing user: {Id}", request.Id);
            return Result<string>.Failure([new Error("Update-User", "User doesn't exist")]);
        }
        if(request.UserUpdate.FirstName is not null)
        {
            currentUser.FirstName=request.UserUpdate.FirstName;
        }
        if(request.UserUpdate.LastName is not null)
        {
            currentUser.LastName=request.UserUpdate.LastName;
        }
        if(request.UserUpdate.PhoneNumber is not null)
        {
            currentUser.Email=request.UserUpdate.PhoneNumber;
        }

        return await identityService.UpdateAsync(currentUser);
    }
}