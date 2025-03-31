using Instructo.Application.Abstractions.Messaging;
using Instructo.Domain.Shared;

namespace Instructo.Application.Users.Commands.RegisterUser;

public record RegisterUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string PhoneNumber,
    string Role) : ICommand<Result<string>>;
