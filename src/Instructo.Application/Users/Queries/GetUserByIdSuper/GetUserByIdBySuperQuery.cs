using Instructo.Domain.Dtos.User;
using Instructo.Domain.Shared;

using MediatR;

namespace Instructo.Application.Users.Queries.GetUserByIdSuper;

public record GetUserByIdBySuperQuery(Guid UserId) : IRequest<Result<UserReadSuperDto>>;