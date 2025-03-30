using Instructo.Application.Abstractions.Messaging;
using Instructo.Domain.Shared;

namespace Instructo.Application.Users.CreateUser;

public record RegisterUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string PhoneNumber) : ICommand<Result<string>>;
