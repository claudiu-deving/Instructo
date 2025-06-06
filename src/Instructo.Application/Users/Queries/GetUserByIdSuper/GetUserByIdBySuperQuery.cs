﻿using Domain.Dtos.User;
using Domain.Entities;
using Domain.Shared;

using MediatR;

namespace Application.Users.Queries.GetUserByIdSuper;

public record GetUserByIdBySuperQuery(Guid UserId) : IRequest<Result<ApplicationUser>>;