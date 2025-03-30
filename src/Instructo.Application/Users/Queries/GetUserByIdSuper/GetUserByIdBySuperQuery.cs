using Instructo.Domain.Dtos;
using Instructo.Domain.Shared;

using MediatR;

namespace Instructo.Application.Users.Queries.GetUserByIdSuper;

public record GetUserByIdBySuperQuery(string UserId) : IRequest<Result<UserReadSuperDto>>;