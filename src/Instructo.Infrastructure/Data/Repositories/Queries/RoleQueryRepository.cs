using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Data.SqlClient;

namespace Infrastructure.Data.Repositories.Queries;

public class RoleQueryRepository(string connectionString) : IRoleQueryRepository
{
    public async Task<IEnumerable<ApplicationRole>> Get()
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        const string sql = """"
                            r.Id,
                            r.Name,
                            r.Description
                            FROM Roles as r
                           """";
        return await connection.QueryAsync<ApplicationRole>(sql);
    }
}