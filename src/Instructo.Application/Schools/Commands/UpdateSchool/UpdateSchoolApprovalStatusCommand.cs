using Application.Abstractions.Messaging;

using Domain.Dtos.School;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Schools.Commands.UpdateSchool;

public record UpdateSchoolApprovalStatusCommand(SchoolId School, bool IsApproved) : ICommand<Result<SchoolDetailReadDto?>>;