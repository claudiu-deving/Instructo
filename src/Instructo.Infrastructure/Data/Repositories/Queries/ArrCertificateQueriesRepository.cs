using System.Data.Common;

using Dapper;

using Instructo.Domain.Entities;
using Instructo.Domain.Enums;
using Instructo.Domain.Interfaces;
using Instructo.Domain.Shared;

using Microsoft.Data;
using Microsoft.Data.SqlClient;

namespace Instructo.Infrastructure.Data.Repositories.Queries;

public class ArrCertificateQueriesRepository : IQueryRepository<ArrCertificate, ARRCertificateType>
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    public ArrCertificateQueriesRepository(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider=dbConnectionProvider;
    }
    public async Task<Result<IEnumerable<ArrCertificate>?>> GetAllAsync()
    {
        try
        {
            using var connection = new SqlConnection(_dbConnectionProvider.ConnectionString);
            await connection.OpenAsync();
            var sql = @" SELECT
                        ac.Id,
                        ac.Name,
                        FROM ARRCertificates ac
                        ";
            return Result<IEnumerable<ArrCertificate>?>.Success(await connection.QueryAsync<ArrCertificate>(sql));
        }
        catch(OperationAbortedException ex)
        {
            return Result<IEnumerable<ArrCertificate>?>.Failure([new Error("GetAllArrCertificates-Aborted", ex.Message)]);
        }
        catch(DbException ex)
        {
            return Result<IEnumerable<ArrCertificate>?>.Failure([new Error("GetAllArrCertificates-Db", ex.Message)]);
        }
    }
    public async Task<Result<ArrCertificate?>> GetByIdAsync(ARRCertificateType id)
    {

        try
        {
            using var connection = new SqlConnection(_dbConnectionProvider.ConnectionString);
            var sql = @"
            SELECT ac.Id,
            ac.Name
            FROM ARRCertificates ac
            WHERE ac.Id=@Id";
            var queryResult = await connection.QuerySingleAsync<ArrCertificate>(sql, new { Id = (int)id });
            return Result<ArrCertificate?>.Success(queryResult);
        }
        catch(InvalidOperationException ex)
        {
            return Result<ArrCertificate?>.Failure([new Error("GetArrCertificateById-Empty", ex.Message)]);
        }
        catch(OperationAbortedException ex)
        {
            return Result<ArrCertificate?>.Failure([new Error("GetArrCertificateById-Aborted", ex.Message)]);
        }
        catch(DbException ex)
        {
            return Result<ArrCertificate?>.Failure([new Error("GetArrCertificateById-Db", ex.Message)]);
        }
    }

    public Task<Result<IEnumerable<ArrCertificate>?>> GetByIndexed(string indexValue)
    {
        throw new NotImplementedException();
    }
}
