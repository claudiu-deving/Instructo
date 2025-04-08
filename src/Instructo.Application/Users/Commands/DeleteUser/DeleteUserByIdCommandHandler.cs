using Application.Abstractions.Messaging;

using Domain.Interfaces;
using Domain.Shared;

using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.DeleteUser;

public class DeleteUserByIdCommandHandler(IIdentityService identityService, ILogger<DeleteUserByIdCommandHandler> logger) : ICommandHandler<DeleteUserByIdCommand, Result<string>>
{
    public async Task<Result<string>> Handle(DeleteUserByIdCommand request, CancellationToken cancellationToken)
    {
        var user = await identityService.GetUserByIdAsync(request.Id);
        if(user==null)
        {
            logger.LogError("Delete user request for non-existing user: {Id}", request.Id);
            return Result<string>.Failure([new Error("Delete-User", "User doesn't exist")]);
        }
        return await identityService.DeleteAsync(user);
    }
}