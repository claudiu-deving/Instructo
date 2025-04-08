using System.Data.Common;

using Dapper;

using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Shared;

using Microsoft.Data;
using Microsoft.Data.SqlClient;

namespace Infrastructure.Data.Repositories.Queries;

public class VehicleCategoryQueriesRepository : IQueryRepository<VehicleCategory, VehicleCategoryType>
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    public VehicleCategoryQueriesRepository(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider=dbConnectionProvider;
    }
    public async Task<Result<IEnumerable<VehicleCategory>?>> GetAllAsync()
    {
        try
        {
            using var connection = new SqlConnection(_dbConnectionProvider.ConnectionString);
            await connection.OpenAsync();
            var sql = @" SELECT
                        vc.Id,
                        vc.Name,
                        vc.Description
                        FROM VehicleCategories vc
                        ";
            return Result<IEnumerable<VehicleCategory>?>.Success(await connection.QueryAsync<VehicleCategory>(sql));
        }
        catch(OperationAbortedException ex)
        {
            return Result<IEnumerable<VehicleCategory>?>.Failure([new Error("GetAllVehicleCategories-Aborted", ex.Message)]);
        }
        catch(DbException ex)
        {
            return Result<IEnumerable<VehicleCategory>?>.Failure([new Error("GetAllVehicleCategories-Db", ex.Message)]);
        }
    }
    public async Task<Result<VehicleCategory?>> GetByIdAsync(VehicleCategoryType id)
    {

        try
        {
            using var connection = new SqlConnection(_dbConnectionProvider.ConnectionString);
            var sql = @"
            SELECT vc.Id,
            vc.Name,
            vc.Description
            FROM VehicleCategories vc
            WHERE vc.Id=@Id";
            var queryResult = await connection.QuerySingleAsync<VehicleCategory>(sql, new { Id = (int)id });
            return Result<VehicleCategory?>.Success(queryResult);
        }
        catch(InvalidOperationException ex)
        {
            return Result<VehicleCategory?>.Failure([new Error("GetVehicleCategoryById-Empty", ex.Message)]);
        }
        catch(OperationAbortedException ex)
        {
            return Result<VehicleCategory?>.Failure([new Error("GetVehicleCategoryById-Aborted", ex.Message)]);
        }
        catch(DbException ex)
        {
            return Result<VehicleCategory?>.Failure([new Error("GetVehicleCategoryById-Db", ex.Message)]);
        }
    }

    public Task<Result<IEnumerable<VehicleCategory>?>> GetByIndexed(string indexValue)
    {
        throw new NotImplementedException();
    }
}
