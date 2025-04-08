using System.Data;
using System.Data.Common;

using Dapper;

using Domain.Entities;
using Domain.Entities.SchoolEntities;
using Domain.Interfaces;
using Domain.Shared;
using Domain.ValueObjects;

using Infrastructure.Data.Repositories.Queries.TypeHandlers;

using Microsoft.Data;
using Microsoft.Data.SqlClient;

using Newtonsoft.Json;

namespace Infrastructure.Data.Repositories.Queries;

public class SchoolQueriesRepository : IQueryRepository<School, SchoolId>
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    public SchoolQueriesRepository(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider=dbConnectionProvider;
        SqlMapper.AddTypeHandler(new SchoolIdTypeHandler());
        SqlMapper.AddTypeHandler(new SchoolNameTypeHandler());
        SqlMapper.AddTypeHandler(new LegalNameTypeHandler());
        SqlMapper.AddTypeHandler(new EmailTypeHandler());
        SqlMapper.AddTypeHandler(new PhoneNumberTypeHandler());
        SqlMapper.AddTypeHandler(new WebsiteLinkNameTypeHandler());
        SqlMapper.AddTypeHandler(new UrlTypeHandler());
        SqlMapper.AddTypeHandler(new BussinessHoursTypeHandler());
        SqlMapper.AddTypeHandler(new PhoneNumberGroupsTypeHandler());
    }

    public async Task<Result<IEnumerable<School>?>> GetAllAsync()
    {
        try
        {

            using var connection = new SqlConnection(_dbConnectionProvider.ConnectionString);
            await connection.OpenAsync();
            var getSchoolSql = @"
            SELECT 
            s.Id,
            s.Name,
            s.CompanyName,
            u.Email,
            s.PhoneNumber,
            s.BussinessHours
            FROM Schools s
            JOIN Users u on u.Id = s.OwnerId";
            var queryResult = await connection.QueryAsync<School>(getSchoolSql);
            var getLinksSql = @"
            SELECT 
            w.Url,
            w.Name,
            w.Description
            From WebsiteLinks w
            JOIN SchoolWebsiteLinks swl on swl.WebsiteLinksId = w.Id
            WHERE swl.SchoolsId = @Id
            ";
            foreach(var school in queryResult)
            {
                var websiteLinks = await connection.QueryAsync<WebsiteLink>(getLinksSql, new { school.Id.Id });
                foreach(var link in websiteLinks)
                {
                    school.AddLink(link);
                }
            }

            var getVehicleCategoriesSql = @"
            SELECT
            vc.Id,
            vc.Name,
            vc.Description
            FROM VehicleCategories vc
            JOIN SchoolCategories svc on svc.VehicleCategoriesId = vc.Id
            WHERE svc.SchoolsId = @Id";

            foreach(var school in queryResult)
            {
                var vehicleCategories = await connection.QueryAsync<VehicleCategory>(getVehicleCategoriesSql, new { school.Id.Id });
                foreach(var category in vehicleCategories)
                {
                    school.AddVehicleCategory(category);
                }
            }

            var getCertificatesSql = @"
            SELECT
            c.Id,
            c.Name,
            c.Description
            FROM ARRCertificates c
            JOIN SchoolCertificates sc on sc.CertificatesId = c.Id
            WHERE sc.SchoolsId = @Id";
            foreach(var school in queryResult)
            {
                var certificates = await connection.QueryAsync<ArrCertificate>(getCertificatesSql, new { school.Id.Id });
                foreach(var certificate in certificates)
                {
                    school.AddCertificate(certificate);
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
            using var connection = new SqlConnection(_dbConnectionProvider.ConnectionString);
            var sql = @"
            SELECT s.Id,
            s.Name,
            s.CompanyName,
            u.Id as OwnerId
            FROM Schools s
            JOIN Users u on s.OwnerId = u.Id
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

    public async Task<Result<IEnumerable<School>?>> GetByIndexed(string value)
    {
        using var connection = new SqlConnection(_dbConnectionProvider.ConnectionString);
        var sql = @$"
            SELECT s.Id,
            s.Name,
            s.CompanyName
            FROM Schools s
            WHERE s.CompanyName=@value";
        var queryResult = await connection.QueryAsync<School>(sql, new { value });

        return Result<IEnumerable<School>?>.Success(queryResult);
    }
    /// <summary>
    /// Custom type handler for SchoolId.
    /// </summary>
    private class SchoolIdTypeHandler : SqlMapper.TypeHandler<SchoolId>
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
    /// <summary>
    /// Custom type handler for SchoolName
    /// </summary>
    private class SchoolNameTypeHandler : SqlMapper.TypeHandler<SchoolName>
    {
        public override SchoolName Parse(object value)
        {
            return value is string name ? SchoolName.Wrap(name) : default;
        }
        public override void SetValue(IDbDataParameter parameter, SchoolName value)
        {
            parameter.Value=value.Value;
        }
    }

    /// <summary>
    /// Custom type handler for LegalName
    /// </summary>
    private class LegalNameTypeHandler : SqlMapper.TypeHandler<LegalName>
    {
        public override LegalName Parse(object value)
        {
            return value is string name ? LegalName.Wrap(name) : default;
        }
        public override void SetValue(IDbDataParameter parameter, LegalName value)
        {
            parameter.Value=value.Value;
        }
    }

    private class WebsiteLinkNameTypeHandler : SqlMapper.TypeHandler<WebsiteLinkName>
    {
        public override WebsiteLinkName Parse(object value)
        {
            return value is string name ? WebsiteLinkName.Wrap(name) : default;
        }
        public override void SetValue(IDbDataParameter parameter, WebsiteLinkName value)
        {
            parameter.Value=value.Value;
        }
    }

    private class BussinessHoursTypeHandler : SqlMapper.TypeHandler<BussinessHours>
    {
        public override BussinessHours Parse(object value)
        {
            return value is string json ? JsonConvert.DeserializeObject<BussinessHours>(json) : BussinessHours.Empty;
        }
        public override void SetValue(IDbDataParameter parameter, BussinessHours? value)
        {
            parameter.Value=JsonConvert.SerializeObject(value);
        }
    }

    private class PhoneNumberGroupsTypeHandler : SqlMapper.TypeHandler<PhoneNumbersGroup>
    {
        public override PhoneNumbersGroup Parse(object value)
        {
            return value is string json ? JsonConvert.DeserializeObject<PhoneNumbersGroup>(json) : PhoneNumbersGroup.Empty;
        }
        public override void SetValue(IDbDataParameter parameter, PhoneNumbersGroup? value)
        {
            parameter.Value=JsonConvert.SerializeObject(value);
        }
    }
}