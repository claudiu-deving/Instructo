using Application.Abstractions.Messaging;

using Domain.Shared;

namespace Application.Users.Commands.LoginUser;

public record LoginUserCommand(string Email, string Password) : ICommand<Result<string>>;