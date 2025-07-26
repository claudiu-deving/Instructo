using System.Data.Common;

using Domain.Entities;
using Domain.Interfaces;
using Domain.Shared;

using Microsoft.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories.Queries;

public class InstructorProfileQueryRepository(AppDbContext appDbContext) : IInstructorProfileQueryRepository
{
    public async Task<Result<IEnumerable<InstructorProfile>>> GetAllAsync()
    {
        try
        {
            return await appDbContext.Instructors.ToListAsync();
        }
        catch(OperationAbortedException ex)
        {
            return new Error("GetAllInstructorProfilees-Aborted", ex.Message);
        }
        catch(DbException ex)
        {
            return new Error("GetAllInstructorProfilees-Db", ex.Message);
        }
    }

    public async Task<Result<InstructorProfile?>> GetByIdAsync(Guid id)
    {
        try
        {
            return await appDbContext.Instructors.FirstOrDefaultAsync(x => x.Id==id);
        }
        catch(InvalidOperationException ex)
        {
            return new Error("GetInstructorProfileById-Empty", ex.Message);
        }
        catch(OperationAbortedException ex)
        {
            return new Error("GetInstructorProfileById-Aborted", ex.Message);
        }
        catch(DbException ex)
        {
            return new Error("GetInstructorProfileById-Db", ex.Message);
        }
    }

    public async Task<Result<InstructorProfile?>> GetByIndexed(string indexedValue)
    {
        try
        {
            return await appDbContext.Instructors.FirstOrDefaultAsync(x => x.TeamId.ToString()==indexedValue);
        }
        catch(OperationAbortedException ex)
        {
            return new Error("GetInstructorProfileByName-Aborted", ex.Message);
        }
        catch(DbException ex)
        {
            return new Error("GetInstructorProfileByName-Db", ex.Message);
        }
    }

    public async Task<Result<IEnumerable<InstructorProfile>>> GetBySchoolIdAsync(Guid schoolId)
    {
        try
        {
            return await appDbContext.Instructors
                .Include(x => x.Team)
                .Where(x => x.Team.SchoolId==schoolId)
                .ToListAsync();
        }
        catch(OperationAbortedException ex)
        {
            return new Error("GetInstructorProfileBySchoolId-Aborted", ex.Message);
        }
        catch(DbException ex)
        {
            return new Error("GetInstructorProfileBySchoolId-Db", ex.Message);
        }
    }
}
