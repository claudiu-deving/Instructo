using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Instructo.Domain.Dtos;
using Instructo.Domain.Shared;

using MediatR;

namespace Instructo.Application.Users.Queries.GetUsersBySuper;

public class GetUsersBySuperQuery : IRequest<Result<IEnumerable<UserReadSuperDto>>>
{

}
