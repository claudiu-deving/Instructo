using Application.Abstractions.Messaging;

using Domain.Dtos.User;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Shared;

namespace Application.Users.Queries.GetUserByEmail;

public class GetUserByEmailQueryHandler : ICommandHandler<GetUserByEmailQuery, Result<ApplicationUser>>
{
    private readonly IUserQueriesRepository _userQueries;
    public GetUserByEmailQueryHandler(IUserQueriesRepository userQueries)
    {
        _userQueries=userQueries;
    }

    public async Task<Result<ApplicationUser>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await _userQueries.GetUserByEmailAsync(request.Email);
        if(user==null)
            return new Error[] { new Error(request.Email, "User not found") };
        return user;
    }
}