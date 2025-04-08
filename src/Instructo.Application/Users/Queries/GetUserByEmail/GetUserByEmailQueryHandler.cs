using Application.Abstractions.Messaging;

using Domain.Dtos.User;
using Domain.Interfaces;
using Domain.Shared;

namespace Application.Users.Queries.GetUserByEmail;

public class GetUserByEmailQueryHandler : ICommandHandler<GetUserByEmailQuery, Result<UserReadDto>>
{
    private readonly IUserQueries _userQueries;
    public GetUserByEmailQueryHandler(IUserQueries userQueries)
    {
        _userQueries=userQueries;
    }

    public async Task<Result<UserReadDto>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await _userQueries.GetUserByEmailAsync(request.Email);
        if(user==null)
            return new Error[] { new Error(request.Email, "User not found") };
        return user;
    }
}