using Application.Abstractions.Messaging;

using Domain.Shared;
using Domain.ValueObjects;

using Messager;

namespace Application.Schools.Commands.DeleteSchool;

public record DeleteSchoolCommand(SchoolId Id, string UserId) : ICommand<Result<Unit>>;