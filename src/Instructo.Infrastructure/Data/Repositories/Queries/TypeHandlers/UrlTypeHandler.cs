using System.Data;

using Dapper;

using Domain.ValueObjects;

namespace Infrastructure.Data.Repositories.Queries.TypeHandlers;

internal class UrlTypeHandler : SqlMapper.TypeHandler<Url>
{
    public override Url Parse(object value)
    {
        return value is string name ? Url.Wrap(name) : default;
    }

    public override void SetValue(IDbDataParameter parameter, Url value)
    {
        parameter.Value=value.Value;
    }
}
