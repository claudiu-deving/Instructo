using System.Data;
using System.Data.Common;

using Dapper;

using Instructo.Domain.Entities;
using Instructo.Domain.Interfaces;
using Instructo.Domain.Shared;
using Instructo.Domain.ValueObjects;

using Microsoft.Data;
using Microsoft.Data.SqlClient;

namespace Instructo.Infrastructure.Data.Repositories.Queries;

public class SchoolQueriesRepository(IDbConnectionProvider dbConnectionProvider) : IQueryRepository<School, SchoolId>
{
    public async Task<Result<IEnumerable<School>?>> GetAllAsync()
    {
        try
        {
            SqlMapper.AddTypeHandler(new SchoolIdTypeHandler());
            using var connection = new SqlConnection(dbConnectionProvider.ConnectionString);
            await connection.OpenAsync();
            var getSchoolSql = @"
            SELECT s.Id,
            s.Name,
            s.CompanyName,
            u.Email
            FROM Schools s
            JOIN Users u on u.Id = s.OwnerId";
            var queryResult = await connection.QueryAsync<School>(getSchoolSql);
            var getLinksSql = @"
            SELECT w.Url,
            w.Name,
            w.Description
            From WebsiteLinks w
            JOIN SchoolWebsiteLinks swl on swl.WebsiteLinksId = w.Id
            WHERE swl.SchoolsId = @Id
            ";
            foreach(School school in queryResult)
            {
                var websiteLinks = await connection.QueryAsync<WebsiteLink>(getLinksSql,new { Id = school.Id.Id });
                foreach(var link in websiteLinks)
                {
                    school.AddLink(link);
                }
            }

            return Result<IEnumerable<School>?>.Success(queryResult);

        }
        catch(OperationAbortedException ex)
        {
            return Result<IEnumerable<School>?>.Failure([new Error("GetAllSchools-Aborted", ex.Message)]);
        }
        catch(DbException ex)
        {
            return Result<IEnumerable<School>?>.Failure([new Error("GetAllSchools-Db", ex.Message)]);
        }
    }

    public async Task<Result<School?>> GetByIdAsync(SchoolId id)
    {
        try
        {
            using var connection = new SqlConnection(dbConnectionProvider.ConnectionString);
            var sql = @"
            SELECT s.Id,
            s.Name,
            s.CompanyName
            FROM Schools s
            WHERE s.Id=@Id";
            var queryResult = await connection.QuerySingleAsync<School>(sql, new { id.Id });
            return Result<School?>.Success(queryResult);
        }
        catch(InvalidOperationException ex)
        {
            return Result<School?>.Failure([new Error("GetSchoolById-Empty", ex.Message)]);
        }
        catch(OperationAbortedException ex)
        {
            return Result<School?>.Failure([new Error("GetSchoolById-Aborted", ex.Message)]);
        }
        catch(DbException ex)
        {
            return Result<School?>.Failure([new Error("GetSchoolById-Db", ex.Message)]);
        }
    }
}
public class SchoolIdTypeHandler : SqlMapper.TypeHandler<SchoolId>
{
    public override SchoolId Parse(object value)
    {
        return value is Guid guid ? SchoolId.CreateNew(guid) : default;
    }

    public override void SetValue(IDbDataParameter parameter, SchoolId value)
    {
        parameter.Value=value.Id;
    }
}
