using System.Data.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Shared;
using Microsoft.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories.Queries;

public class ArrCertificateQueriesRepository : IQueryRepository<ArrCertificate, ARRCertificateType>
{
    private readonly AppDbContext _dbContext;

    public ArrCertificateQueriesRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<IEnumerable<ArrCertificate>?>> GetAllAsync()
    {
        try
        {
            return Result<IEnumerable<ArrCertificate>?>.Success(await _dbContext.CertificateTypes.ToListAsync());
        }
        catch (OperationAbortedException ex)
        {
            return Result<IEnumerable<ArrCertificate>?>.Failure(new Error("GetAllArrCertificates-Aborted", ex.Message));
        }
        catch (DbException ex)
        {
            return Result<IEnumerable<ArrCertificate>?>.Failure(new Error("GetAllArrCertificates-Db", ex.Message));
        }
    }

    public async Task<Result<ArrCertificate?>> GetByIdAsync(ARRCertificateType id)
    {
        try
        {
            return Result<ArrCertificate?>.Success(await _dbContext.CertificateTypes.FindAsync(id));
        }
        catch (InvalidOperationException ex)
        {
            return Result<ArrCertificate?>.Failure(new Error("GetArrCertificateById-Empty", ex.Message));
        }
        catch (OperationAbortedException ex)
        {
            return Result<ArrCertificate?>.Failure(new Error("GetArrCertificateById-Aborted", ex.Message));
        }
        catch (DbException ex)
        {
            return Result<ArrCertificate?>.Failure(new Error("GetArrCertificateById-Db", ex.Message));
        }
    }

    public Task<Result<ArrCertificate>?> GetByIndexed(string companyName)
    {
        throw new NotImplementedException();
    }
}