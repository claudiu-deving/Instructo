using Instructo.Domain.Dtos.User;
using Instructo.Domain.Interfaces;
using Instructo.Domain.Shared;

using MediatR;

namespace Instructo.Application.Users.Queries.GetUsersBySuper;

public class GetUsersBySuperQueryHandler : IRequestHandler<GetUsersBySuperQuery, Result<IEnumerable<UserReadSuperDto>>>
{
    private readonly IUserQueries _instructorQueries;

    public GetUsersBySuperQueryHandler(IUserQueries instructorQueries)
    {
        _instructorQueries=instructorQueries;
    }
    public async Task<Result<IEnumerable<UserReadSuperDto>>> Handle(GetUsersBySuperQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var users = await _instructorQueries.GetUsersBySuper();
            return Result<IEnumerable<UserReadSuperDto>>.Success(users);
        }
        catch(Exception ex)
        {
            return Result<IEnumerable<UserReadSuperDto>>.Failure([new Error("Error", ex.Message)]);
        }
    }
}