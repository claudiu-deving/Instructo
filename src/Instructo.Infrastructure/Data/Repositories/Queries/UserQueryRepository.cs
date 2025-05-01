using System.Data;
using Dapper;

using Domain.Dtos.User;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ValueObjects;
using Microsoft.Data.SqlClient;

namespace Infrastructure.Data.Repositories.Queries;

public class UserQueryRepository : IUserQueries
{
    private readonly string _connectionString;

    public UserQueryRepository(string connectionString)
    {
        _connectionString = connectionString;
        SqlMapper.AddTypeHandler(new RoleTypeHandler());
    }

    internal class RoleTypeHandler : SqlMapper.TypeHandler<ApplicationRole>
    {
        public override ApplicationRole Parse(object value)
        {
            return value is string name ? new ApplicationRole(){Name = name}: default;
        }

        public override void SetValue(IDbDataParameter parameter, ApplicationRole value)
        {
            parameter.Value=value.Name;
        }
    }

    public async Task<ApplicationUser> GetUserByEmailAsync(string email)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = """

                                       SELECT u.Id,
                                       u.FirstName,
                                       u.Email,
                                       u.LastName,
                                       r.Name as Role
                                       FROM Users u
                                       JOIN UserRoles ur ON ur.UserId = u.Id
                                       JOIN Roles r ON r.Id = ur.RoleId
                                       WHERE u.Email = @email
                           """;
        return await connection.QueryFirstAsync<ApplicationUser>(sql, new { email });
    }

    public async Task<IEnumerable<ApplicationUser>> GetUsers()
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = """

                                       SELECT i.Id,
                                       i.FirstName,
                                       i.Email,
                                       i.LastName,
                                       r.Name as Role
                                       FROM Users i
                                       JOIN UserRoles ur ON ur.UserId = i.Id
                                       JOIN Roles r ON r.Id = ur.RoleId
                           """;
        return await connection.QueryAsync<ApplicationUser>(sql);
    }

    public async Task<ApplicationUser> GetUsersByIdAsync(Guid userId)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = """
                                       SELECT i.Id,
                                       i.FirstName,
                                       i.Email,
                                       i.LastName,
                                       r.Name as Role
                                       FROM Users i
                                       JOIN UserRoles ur ON ur.UserId = i.Id
                                       JOIN Roles r ON r.Id = ur.RoleId
                                       WHERE i.Id = @userId
                           """;
        return await connection.QueryFirstAsync<ApplicationUser>(sql, userId);
    }

    public async Task<ApplicationUser?> GetUsersByIdBySuperAsync(Guid userId)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = """
                                       SELECT  i.Id,
                                       i.FirstName,
                                       i.Email,
                                       i.LastName,
                                       r.Name as Role,
                                       i.Created as CreatedUtc,
                                       i.LastLogin as LastLoginUtc,
                                       i.IsActive,
                                       i.EmailConfirmed as IsEmailConfirmed,
                                       i.PhoneNumber,
                                       i.PhoneNumberConfirmed as IsPhoneConfirmed,
                                       i.TwoFactorEnabled as IsTwoFactorEnabled
                                       FROM Users i
                                       JOIN UserRoles ur ON ur.UserId = i.Id
                                       JOIN Roles r ON r.Id = ur.RoleId
                                       WHERE i.Id = @id
                           """;
        return await connection.QueryFirstAsync<ApplicationUser?>(sql, new { id = userId });
    }

    public async Task<IEnumerable<ApplicationUser>> GetUsersBySuper()
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = """
                                       SELECT i.Id,
                                       i.FirstName,
                                       i.Email,
                                       i.LastName,
                                       r.Name as Role,
                                       i.Created as CreatedUtc,
                                       i.LastLogin as LastLoginUtc,
                                       i.IsActive,
                                       i.EmailConfirmed as IsEmailConfirmed,
                                       i.PhoneNumber,
                                       i.PhoneNumberConfirmed as IsPhoneConfirmed,
                                       i.TwoFactorEnabled as IsTwoFactorEnabled
                                       FROM Users i
                                       JOIN UserRoles ur ON ur.UserId = i.Id
                                       JOIN Roles r ON r.Id = ur.RoleId
                           """;
        return await connection.QueryAsync<ApplicationUser>(sql);
    }

    public async Task<bool> IsEmailUnique(string email)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
        var count = await connection.ExecuteScalarAsync<int>(sql, new { email });
        return count==0;
    }
}
