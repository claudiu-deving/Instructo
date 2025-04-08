using System.Data;

using Dapper;

using Domain.ValueObjects;

namespace Infrastructure.Data.Repositories.Queries.TypeHandlers;

internal class PhoneNumberTypeHandler : SqlMapper.TypeHandler<PhoneNumber>
{
    public override PhoneNumber Parse(object value)
    {
        return value is string name ? PhoneNumber.Wrap(name) : PhoneNumber.Empty;
    }

    public override void SetValue(IDbDataParameter parameter, PhoneNumber? value)
    {
        parameter.Value=value?.Value;
    }
}
