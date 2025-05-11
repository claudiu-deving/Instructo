using Application.Abstractions.Messaging;
using Application.Users.Queries.GetUserById;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Shared;

namespace Application.Users.Queries.GetUserByEmail;

public class GetUserByIdQueryHandler : ICommandHandler<GetUserByIdQuery, Result<ApplicationUser>>
{
    private readonly IUserQueries _userQueries;

    public GetUserByIdQueryHandler(IUserQueries userQueries)
    {
        _userQueries = userQueries;
    }

    public async Task<Result<ApplicationUser>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userQueries.GetUserByIdAsync(request.UserId);
        if (user == null)
            return new[] { new Error(request.UserId.ToString(), "User not found") };
        return user;
    }
}