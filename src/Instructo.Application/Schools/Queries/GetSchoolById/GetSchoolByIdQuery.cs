using Application.Abstractions.Messaging;

using Domain.Dtos.School;
using Domain.Shared;

namespace Application.Schools.Queries.GetSchoolById;

public record GetSchoolByIdQuery(Guid Id) : ICommand<Result<SchoolReadDto>>;
