using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instructo.Application.Schools.Queries.GetSchools;

public readonly record struct GetSchoolsQueryParameters(string? SearchTerm, int PageNumber = 1, int PageSize = 50);
