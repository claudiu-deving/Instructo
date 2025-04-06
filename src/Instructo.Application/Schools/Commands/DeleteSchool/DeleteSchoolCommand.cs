using Instructo.Application.Abstractions.Messaging;
using Instructo.Domain.Shared;
using Instructo.Domain.ValueObjects;

using MediatR;

namespace Instructo.Application.Schools.Commands.DeleteSchool;

public record DeleteSchoolCommand(SchoolId Id, string UserId) : ICommand<Result<Unit>>;