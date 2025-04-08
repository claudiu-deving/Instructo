using Application.Abstractions.Messaging;

using Domain.Dtos.School;
using Domain.Shared;

namespace Application.Schools.Queries.GetSchools;

public class GetSchoolsQuery : ICommand<Result<IEnumerable<SchoolReadDto>>>;
