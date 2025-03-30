using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instructo.Application.Users.Queries.GetUsersBySuper;

public readonly record struct GetUsersQueryParameters( string? SearchTerm, string? Role, bool? IsActive, int PageNumber = 1, int PageSize = 50);
