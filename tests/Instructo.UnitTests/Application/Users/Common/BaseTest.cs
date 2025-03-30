using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instructo.UnitTests.Application.Users.Common;

public abstract class BaseTest
{
    protected UserBuilder UserBuilder { get; } = new UserBuilder();
}
