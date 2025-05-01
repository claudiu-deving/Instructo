using Domain.Dtos.User;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Shared;

using MediatR;

namespace Application.Users.Queries.GetUserByIdSuper;

public class GetUserByIdBySuperQueryHandler(IUserQueries instructorQueries)
    : IRequestHandler<GetUserByIdBySuperQuery, Result<ApplicationUser>>
{
    public async Task<Result<ApplicationUser>> Handle(GetUserByIdBySuperQuery request, CancellationToken cancellationToken)
    {
        var user = await instructorQueries.GetUsersByIdBySuperAsync(request.UserId);
        return user is null ? Result<ApplicationUser>.Failure([new Error("Not found", "User not found")]) : Result<ApplicationUser>.Success(user!);
    }
}