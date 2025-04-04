using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper;

using Instructo.Domain.Dtos;
using Instructo.Domain.Interfaces;
using Instructo.Domain.Shared;

using Microsoft.Data.SqlClient;

namespace Instructo.Infrastructure.Data.Repositories.Queries;

public class UserQueryRepository : IUserQueries
{
    private readonly string _connectionString;

    public UserQueryRepository(string connectionString)
    {
        _connectionString=connectionString;
    }

    public async Task<UserReadDto> GetUserByEmailAsync(string email)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var sql = @"
            SELECT u.Id,
            u.FirstName,
            u.Email,
            u.LastName,
            r.Name as Role
            FROM Users u
            JOIN UserRoles ur ON ur.UserId = u.Id
            JOIN Roles r ON r.Id = ur.RoleId
            WHERE u.Email = @email";
        return await connection.QueryFirstAsync<UserReadDto>(sql, new { email });
    }

    public async Task<IEnumerable<UserReadDto>> GetUsers()
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var sql = @"
            SELECT i.Id,
            i.FirstName,
            i.Email,
            i.LastName,
            r.Name as Role
            FROM Users i
            JOIN UserRoles ur ON ur.UserId = i.Id
            JOIN Roles r ON r.Id = ur.RoleId";
        return await connection.QueryAsync<UserReadDto>(sql);
    }

    public async Task<UserReadDto> GetUsersByIdAsync(string userId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var sql = @"
            SELECT i.Id,
            i.FirstName,
            i.Email,
            i.LastName,
            r.Name as Role
            FROM Users i
            JOIN UserRoles ur ON ur.UserId = i.Id
            JOIN Roles r ON r.Id = ur.RoleId
            WHERE i.Id = @userId";
        return await connection.QueryFirstAsync<UserReadDto>(sql, userId);
    }

    public async Task<UserReadSuperDto> GetUsersByIdBySuperAsync(string userId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var sql = @"
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
            WHERE i.Id = @id";
        return await connection.QueryFirstAsync<UserReadSuperDto>(sql, new { id = userId });
    }

    public async Task<IEnumerable<UserReadSuperDto>> GetUsersBySuper()
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var sql = @"
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
            JOIN Roles r ON r.Id = ur.RoleId";
        return await connection.QueryAsync<UserReadSuperDto>(sql);
    }

    public async Task<bool> IsEmailUnique(string email)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var sql = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
        var count = await connection.ExecuteScalarAsync<int>(sql, new { email });
        return count==0;
    }
}
