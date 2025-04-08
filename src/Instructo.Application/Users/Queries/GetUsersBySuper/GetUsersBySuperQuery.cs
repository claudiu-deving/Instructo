using Domain.Dtos.User;
using Domain.Shared;

using MediatR;

namespace Application.Users.Queries.GetUsersBySuper;

public class GetUsersBySuperQuery : IRequest<Result<IEnumerable<UserReadSuperDto>>>
{

}
