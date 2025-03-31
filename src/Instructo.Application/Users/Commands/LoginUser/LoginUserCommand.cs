using Instructo.Application.Abstractions.Messaging;
using Instructo.Domain.Shared;

namespace Instructo.Application.Users.Commands.LoginUser;

public record LoginUserCommand(string Email, string Password) : ICommand<Result<string>>;