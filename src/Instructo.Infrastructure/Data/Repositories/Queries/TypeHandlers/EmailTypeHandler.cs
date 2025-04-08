using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper;

using Domain.ValueObjects;

namespace Infrastructure.Data.Repositories.Queries.TypeHandlers;

internal class EmailTypeHandler : SqlMapper.TypeHandler<Email>
{
    public override Email Parse(object value)
    {
        return value is string name ? Email.Wrap(name) : default;
    }

    public override void SetValue(IDbDataParameter parameter, Email value)
    {
        parameter.Value=value.Value;
    }
}
