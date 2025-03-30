using Instructo.Domain.Dtos;
using Instructo.Domain.Interfaces;
using Instructo.Domain.Shared;

using MediatR;

namespace Instructo.Application.Users.Queries.GetUserByIdSuper;

public class GetUserByIdBySuperQueryHandler : IRequestHandler<GetUserByIdBySuperQuery, Result<UserReadSuperDto>>
{
    private readonly IUserQueries _userQueries;

    public GetUserByIdBySuperQueryHandler(IUserQueries instructorQueries)
    {
        _userQueries=instructorQueries;
    }

    public async Task<Result<UserReadSuperDto>> Handle(GetUserByIdBySuperQuery request, CancellationToken cancellationToken)
    {
        var user = await _userQueries.GetUsersByIdBySuperAsync(request.UserId);
        if(user==null)
            return Result<UserReadSuperDto>.Failure([new Error("Not found", "User not found")]);
        else
        {
            return Result<UserReadSuperDto>.Success(user);
        }
    }
}