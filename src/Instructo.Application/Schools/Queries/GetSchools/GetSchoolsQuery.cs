using Instructo.Application.Abstractions.Messaging;
using Instructo.Domain.Dtos;
using Instructo.Domain.Shared;

namespace Instructo.Application.Schools.Queries.GetSchools;

public class GetSchoolsQuery : ICommand<Result<IEnumerable<SchoolReadDto>>>;
