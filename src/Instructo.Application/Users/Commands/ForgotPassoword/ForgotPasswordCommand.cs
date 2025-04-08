using Application.Abstractions.Messaging;

using Domain.Shared;

namespace Application.Users.Commands.ForgotPassoword;

public record ForgotPasswordCommand(string Email) : ICommand<Result<string>>;
