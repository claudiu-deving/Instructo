using Instructo.Domain.Dtos;
using Instructo.Domain.Shared;

using MediatR;

namespace Instructo.Application.Users.Query;

public record GetUserByIdBySuperQuery(string UserId) : IRequest<Result<UserReadSuperDto>>;