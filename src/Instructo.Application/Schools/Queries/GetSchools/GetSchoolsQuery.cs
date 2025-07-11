﻿using Application.Abstractions.Messaging;
using Domain.Dtos.School;
using Domain.Shared;

namespace Application.Schools.Queries.GetSchools;

public class GetSchoolsQuery(bool IsAdmin) : ICommand<Result<IEnumerable<SchoolReadDto>>>
{
    public bool IsAdmin { get; set; } = IsAdmin;
}