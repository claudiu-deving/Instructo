using Instructo.Application.Abstractions.Messaging;
using Instructo.Domain.Dtos.School;
using Instructo.Domain.Shared;

namespace Instructo.Application.Schools.Queries.GetSchoolById;

public record GetSchoolByIdQuery(Guid Id) : ICommand<Result<SchoolReadDto>>;
