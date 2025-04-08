using Application.Abstractions.Messaging;

using Domain.Shared;

namespace Application.Users.Commands.DeleteUser;

public record DeleteUserByIdCommand(Guid Id) : ICommand<Result<string>>;
