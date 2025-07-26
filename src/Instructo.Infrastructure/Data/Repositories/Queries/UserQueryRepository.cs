using Domain.Entities;
using Domain.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories.Queries;

public class UserQueryRepository(AppDbContext dbContext) : IUserQueriesRepository
{
    public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        return await dbContext.Users.Include(x => x.School).FirstOrDefaultAsync(user => user.Email==email);
    }

    public async Task<IEnumerable<ApplicationUser>> GetUsers()
    {
        var result = await dbContext.Users.ToListAsync();
        return result;
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(Guid userId)
    {
        var userRoles = await dbContext.UserRoles.FirstOrDefaultAsync(x => x.UserId==userId);
        if(userRoles==null)
            return null;
        var role = await dbContext.Roles.FirstOrDefaultAsync(x => x.Id==userRoles.RoleId);

        var user = await dbContext.Users.Include(x => x.School).FirstOrDefaultAsync(user => userId==user.Id);
        if(user==null)
            return null;
        user.Role=role;
        return user;
    }


    public async Task<bool> IsEmailUnique(string email)
    {
        return await dbContext.Users.AnyAsync(x => x.Email==email);
    }
}