using Domain.Interfaces;

namespace Infrastructure.Data;

public class DbConnectionProvider(string connectionString) : IDbConnectionProvider
{
    public string ConnectionString => connectionString;
}
