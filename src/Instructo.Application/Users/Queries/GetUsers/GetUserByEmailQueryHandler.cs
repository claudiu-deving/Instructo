using Application.Abstractions.Messaging;
using Application.Users.Queries.GetUserById;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Shared;

namespace Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler : ICommandHandler<GetUsersQuery, Result<IEnumerable<ApplicationUser>>>
{
    private readonly IUserQueries _userQueries;

    public GetUsersQueryHandler(IUserQueries userQueries)
    {
        _userQueries = userQueries;
    }

    public async Task<Result<IEnumerable<ApplicationUser>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var user = await _userQueries.GetUsers();
        return  Result<IEnumerable<ApplicationUser>>.Success(user); 
    }
}