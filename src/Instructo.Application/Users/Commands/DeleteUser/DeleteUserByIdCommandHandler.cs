using Instructo.Application.Abstractions.Messaging;
using Instructo.Domain.Entities;
using Instructo.Domain.Shared;

using Microsoft.AspNetCore.Identity;

namespace Instructo.Application.Users.Commands.DeleteUser;

public class DeleteUserByIdCommandHandler(UserManager<ApplicationUser> userManager) : ICommandHandler<DeleteUserByIdCommand, Result<string>>
{
    public async Task<Result<string>> Handle(DeleteUserByIdCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Id);
        if(user==null)
            return Result<string>.Failure([new Error("Delete-User", "User doesn't exist")]);
        var result = await userManager.DeleteAsync(user);
        if(result.Succeeded)
            return Result<string>.Success("User deleted");
        else
        {
            return Result<string>.Failure([.. result.Errors.Select(e => new Error(e.Code, e.Description))]);
        }
    }
}