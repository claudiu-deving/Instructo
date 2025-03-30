

using Instructo.Application.Abstractions.Messaging;
using Instructo.Domain.Dtos;
using Instructo.Domain.Shared;

namespace Instructo.Application.Users.UpdateUser;

public record UpdateUserCommand(string Id, UserUpdateDto UserUpdate) : ICommand<Result<string>>;
