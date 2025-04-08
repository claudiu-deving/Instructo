using Application.Abstractions.Messaging;

using Domain.Dtos.User;
using Domain.Shared;

namespace Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(Guid Id, UserUpdateDto UserUpdate) : ICommand<Result<string>>;
